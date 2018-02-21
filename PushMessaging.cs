using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Configuration;
using log4net;
using PushMessagesSender.Logger;

namespace PushMessagesSender
{
    public class PushMessaging
    {
        HttpClientHandler Handler;
        protected ILog Log;      

        public PushMessaging()
        {
            InitLog();
        }

        public PushMessaging(HttpClientHandler handler)
        {
            Handler = handler;
            InitLog();
        }
        
        public void InitLog()
        {
            Log = LogManager.GetLogger("pushMessages.log");
        }

        public string FirebaseAdress = ConfigurationManager.AppSettings["FirebaseAdress"];
        //"https://fcm.googleapis.com/fcm/send"; 

        public string ServerKey = ConfigurationManager.AppSettings["ServerKey"];

        //пример Id устройства (виртуальное устройство в Android Studio)
        public string VirtualDeviceId = "fAQbnjK6a9M:APA91bH1uXCacRZ3wlP-YG86aG8EgsuIrtJgP6OwNHN-Bs7L8bFGalKgz4FCGbXaskN-Ipk1HAxI4HFmh9A7Ahfy8jLZoS_tCJnJ81QorSuLW7CwF5-6ToJpMBszN2fnHAm1BBRH_Hxu";


        public string Topic = ConfigurationManager.AppSettings["Topic"]; //"/topics/news";
        

        /// <summary>
        /// Формирует информационное сообщение для отправки в Firebase
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="preview">Превью сообщения</param>
        /// <param name="sendOptions">Адресат</param>
        /// <returns>Модель push-сообщения в формате для Firebase</returns>
        /// Метод для отправки сообщения, где можно напрямую задать Title и Preview
        public PushMessage FormMessageToFirebase(string title, string preview, SendingOptions sendOptions)
        {
            var message = new PushMessage();
            message.To = GetRecipientString(sendOptions);
            message.Message = new InnerMessage
            {
                Title = title,
                Preview = preview
            };
            return message;
        }
        

        /// <summary>
        /// Формирует строку адресата сообщения
        /// </summary>
        /// <param name="sendOptions"></param>
        /// <returns></returns>
        private string GetRecipientString(SendingOptions sendOptions)
        {
            switch (sendOptions)
            {
                case (SendingOptions.SendToDevice):
                    {
                        return GetDeviceId();
                    }

                case (SendingOptions.SendToTopic):
                    {
                        return GetTopicId();
                    }
                default:
                    return string.Empty;
            }
        }



        /// <summary>
        /// Возвращает Id устройства, на которое уйдет сообщение
        /// </summary>
        /// <returns>Идентификатор устройства для Firebase в виде строки в 152 символа</returns>
        /// Если мы реализуем отправку на определенные устройства, нужно будет
        /// как-то хранить их DeviceId и по каким-то параметрам доставать. Пока - заглушка
        private string GetDeviceId()
        {
            //TODO: поменять! Только на время тестирования!
            return VirtualDeviceId;
        }



        /// <summary>
        /// Возвращает название топика, на которое уйдет сообщение
        /// Строка вернется в виде /topics/TopicName
        /// </summary>
        /// <returns></returns>
        private string GetTopicId()
        {
            return Topic;
        }


        /// <summary>
        /// Отсылает сообщение в Firebase и пишет лог
        /// </summary>
        /// <param name="message"></param>
        public async void SendMessage(PushMessage message)
        {
            try
            {
                Log.Info(LoggerStrings.LOG_SENDINGPUSHMESSAGE(message.Message.Title));
                var response = await SendJSON(message);
                CheckFirebaseResponse(response);
            }
            catch(JsonReaderException readerEx) //пришёл корявый JSON. Такие ошибки надо отлавливать, чтобы быть готовым ко всему, что Firebase нам пошлет
            {
                Log.Error(LoggerStrings.LOG_BADJSON, readerEx);
            }
            catch(Exception ex) //Код ответа != 200 или неизвестная ошибка
            {
                Log.Error(LoggerStrings.LOG_CODENOT200ORUNKNOWNERROR, ex);
            }
            finally
            {
                Log.Error(LoggerStrings.LOG_SEPARATOR);
            }
        }

        /// <summary>
        /// Логгирует ответы сервера Firebase
        /// </summary>
        /// <param name="response"></param>
        /// Подробнее здесь:
        /// https://firebase.google.com/docs/cloud-messaging/server#response
        private void CheckFirebaseResponse(FirebaseResponse response)
        {
            Log.Info(LoggerStrings.LOG_RESPONSERECEIVED);
            if (response.Failure == 0 && (response.Canonical_Ids == null || response.Canonical_Ids == "0"))
            {
                //TODO: что, если вернется несколько messageID?
                Log.Info(LoggerStrings.LOG_MESSAGEOK(response.Message_Id));
            }
            else
            {
                if (response.Results != null)
                {
                    foreach (var result in response.Results)
                    {
                        if (result.Error != null)
                        {
                            Log.Warn(LoggerStrings.LOG_ERRORPUSHINGMESSAGE(result.Error));
                        }
                        else
                        {
                            Log.Warn(LoggerStrings.LOG_UNKNOWNRESULT);
                        }
                    }
                }
            }
 
        }

        /// <summary>
        /// Отсылает заранее сформированное сообщение в Firebase
        /// </summary>
        /// <param name="message"></param>
        private async Task<FirebaseResponse> SendJSON(PushMessage message)
        {
            string json = JsonConvert.SerializeObject(message);

            var client = (Handler == null) ? new HttpClient() : new HttpClient(Handler);

            //Content-Type Header
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            #region Headers

            //Accept
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //Authorization
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", ServerKey);

            #endregion


            var response = client.PostAsync(FirebaseAdress, stringContent, new System.Threading.CancellationToken()).Result;

            var code = response.StatusCode;
            string responseString = await response.Content.ReadAsStringAsync();

            //Если код != 200, случилась какая-то ерунда. Десереализовать объект не получится
            bool code200 = (code == HttpStatusCode.OK);
            //Иногда Firebase возвращает Plain Text вида "Error=InvalidRegistration". Её лучше отловить сразу:
            bool plainTextReturned = (responseString.Contains("Error=") && (responseString.IndexOf("results") == -1));

            if (!code200 || plainTextReturned)
            {
                throw new Exception(responseString);
            }
            
            else
            {
                FirebaseResponse fireResponse = JsonConvert.DeserializeObject<FirebaseResponse>(responseString);
                return fireResponse;
            }
        }      


        #region TestingMethods

        public PushMessage GetSampleMessage(SendingOptions opts)
        {
            var message = new PushMessage();
            message.To = GetRecipientString(opts);
            message.Message = new InnerMessage
            {
                Title = "From C#",
                Preview = "Preview"
            };
            return message;
        }

        #endregion

    }


    /// <summary>
    /// Модель ответа от сервера Firebase
    /// </summary>
    public class SimpleFirebaseResponse
    {
        public HttpStatusCode Code { get; set; }
        public string Body { get; set; }


        public override string ToString()
        {
            string ret = string.Concat(Code.ToString(), Environment.NewLine, Body.ToString());
            return ret;
        }
    }

}
