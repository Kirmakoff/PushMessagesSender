using Newtonsoft.Json;


namespace PushMessagesSender
{
    /// <summary>
    /// Сообщение, которое будет отправлено на Firebase.
    /// </summary>
    public class PushMessage
    {
        /// <summary>
        /// Адресат сообщения
        /// </summary>
        /// Firebase может отправлять сообщения индивидуально на устройства, но там ограничение: 1000/сутки.
        /// Поэтому, скорее всего, мы будем отправлять на topic
        /// Но поддержку индивидуальных сообщений оставляем
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Тело сообщения
        /// </summary>
        /// Это именно уведомление, которое будет показано пользователю.
        /// Еще нам может понадобиться data для отсылки данных самому приложению без уведомления пользователя. 
        /// Поддержки этого вида сообщений пока нет
        [JsonProperty("notification")]
        public InnerMessage Message { get; set; }

    }
}
