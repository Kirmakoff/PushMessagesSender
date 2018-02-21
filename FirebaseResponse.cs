using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PushMessagesSender
{

    /// <summary>
    /// Структура, содержашая все данные, которые Firebase прислал в ответ
    /// </summary>
    class FirebaseResponse
    {
        [JsonProperty("multicast_id")]
        public string Multicast_Id { get; set; }

        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("failure")]
        public int Failure { get; set; }

        [JsonProperty("canonical_ids")]
        public string Canonical_Ids { get; set; }

        [JsonProperty("results")]
        public List<Results> Results { get; set; }

        [JsonProperty("message_id")]
        public string Message_Id { get; set; }

    }

    public class Results
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("message_id")]
        public string Message_Id { get; set; }

        [JsonProperty("registration_id")]
        public string Registration_Id { get; set; }
    }
}

