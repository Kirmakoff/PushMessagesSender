namespace PushMessagesSender.Logger
{

    /// <summary>
    /// Строки для log4net.
    /// </summary>
    /// TODO: По-хорошему он должен сам обращаться к log4net, тогда будет нормальный фасад/wrapper
    /// 1) Пишем Info, когда отправляем сообщение. Также пишем заголовок сообщения.
    /// 2) Если с сообщением всё ок, пишем в Info OK + Message_id.
    /// 3) Если что-то совсем не так (код ответа != 200, или прочая ошибка), пишем Error. Логгируем всё, что можно.
    /// 4) Если пришло 200, но canonical_ids > 0 или Failure != 0, пишем всё, что в Results.
    public static class LoggerStrings
    {
        public static string LOG_PUSHMESSAGES = "Push Messages: ";
        public static string LOG_RESPONSERECEIVED = "Firebase response received";
        public static string LOG_SEPARATOR = "=============";
        public static string LOG_FIREBASEERROR = "Firebase Error: ";
        public static string LOG_UNKNOWNRESULT = "Got unknown result from Firebase";
        public static string LOG_CODENOT200ORUNKNOWNERROR = "Response Code != 200 or unknown error. Exception text: ";
        public static string LOG_BADJSON = "JSON ERROR: ";

        public static string LOG_SENDINGPUSHMESSAGE(string messageTitle)
        {
            return string.Format("Sending push message to Firebase. Message Title: {0}", messageTitle);
        }

        public static string LOG_ERRORPUSHINGMESSAGE(string error)
        {
            return string.Format("Firebase returned 200 with error: {0}}", error);
        }


        public static string LOG_MESSAGEOK(string messageId)
        {
            return string.Format("Firebase response OK. MessageID: {0}", messageId);
        }
    }

}
