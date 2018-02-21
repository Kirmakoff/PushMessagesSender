namespace PushMessagesSender
{
    /// <summary>
    /// Опции посылки push-сообщений
    /// </summary>
    public enum SendingOptions
    {
        /// <summary>
        /// Отправить в топик
        /// </summary>
        SendToTopic,
        /// <summary>
        /// Отправить на определённое устройство
        /// </summary>
        SendToDevice
    }
}
