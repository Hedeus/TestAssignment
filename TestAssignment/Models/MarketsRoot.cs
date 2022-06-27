using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestAssignment.Models
{
    internal class MarketsRoot
    {
        [JsonProperty("markets")]
        public List<Market> Markets { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
