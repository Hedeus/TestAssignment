using Newtonsoft.Json;

namespace TestAssignment.Models
{
    internal class Exchange
    {
        [JsonProperty("exchange_id")]
        public string ExchangeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("website")]
        public string Website{ get; set; }

        [JsonProperty("volume_24h")]
        public double Volume24h{ get; set; }
    }
}
