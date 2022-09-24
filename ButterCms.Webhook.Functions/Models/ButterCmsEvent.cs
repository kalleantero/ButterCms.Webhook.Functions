
using Newtonsoft.Json;

namespace ButterCms.Webhook.Functions.Models
{
    public class Data
    {
        [JsonProperty("id")]
        public string Slug { get; set; }
        [JsonProperty("_id")]
        public int Id { get; set; }
    }

    public class ButterCmsEvent
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
        [JsonProperty("webhook")]
        public Webhook Webhook { get; set; }
    }

    public class Webhook
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
    }
}
