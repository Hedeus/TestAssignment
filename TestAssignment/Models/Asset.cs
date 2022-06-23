using Newtonsoft.Json;

namespace TestAssignment.Models
{
    internal class Asset
    {
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        [JsonProperty("volume_24h")]
        public double Volume24h { get; set; }
        [JsonProperty("change_1h")]
        public double Change1h { get; set; }
        [JsonProperty("change_24h")]
        public double Change24h { get; set; }
        [JsonProperty("change_7d")]
        public double Change7d { get; set; }
    }
}
