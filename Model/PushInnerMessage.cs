using Newtonsoft.Json;

namespace PushMessagesSender
{
    /// <summary>
    /// Внутренняя структура сообщения, которое будет отправлено на устройство.
    /// </summary>
    /// То, что человек увидит на экране: заголовок новости и небольшое превью
    /// обычно превью делают где-то на 40-80 символов. Стоит добавить проверку на это?
    public class InnerMessage
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }
    }
}
