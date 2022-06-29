using Newtonsoft.Json;

namespace TestAssignment.Models
{
    internal class ExchangeRoot
    {
        [JsonProperty("exchange")]
        public Exchange Exchange { get; set; }
    }
}
