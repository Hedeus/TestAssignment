using Newtonsoft.Json;

namespace TestAssignment.Models
{
    internal class Market
    {
        [JsonProperty("exchange_id")]
        public string ExchangeId;

        [JsonProperty("symbol")]
        public string Symbol;

        [JsonProperty("base_asset")]
        public string BaseAsset;

        [JsonProperty("quote_asset")]
        public string QuoteAsset;

        [JsonProperty("price_unconverted")]
        public double PriceUnconverted;

        [JsonProperty("price")]
        public double Price;

        [JsonProperty("change_24h")]
        public double Change24h;

        [JsonProperty("spread")]
        public double Spread;

        [JsonProperty("volume_24h")]
        public double Volume24h;
    }
}
