using Newtonsoft.Json;

namespace TestAssignment.Models
{
    internal class Market
    {
        [JsonProperty("exchange_id")]
        public string ExchangeId { get; set; }

        [JsonProperty("symbol")]
        public string Symbol{ get; set; }

        [JsonProperty("base_asset")]
        public string BaseAsset{ get; set; }

        [JsonProperty("quote_asset")]
        public string QuoteAsset{ get; set; }

        [JsonProperty("price_unconverted")]
        public double PriceUnconverted{ get; set; }

        [JsonProperty("price")]
        public double Price{ get; set; }

        [JsonProperty("change_24h")]
        public double Change24h{ get; set; }

        [JsonProperty("spread")]
        public double Spread{ get; set; }

        [JsonProperty("volume_24h")]
        public double Volume24h{ get; set; }
    }
}
