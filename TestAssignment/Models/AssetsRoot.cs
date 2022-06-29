using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestAssignment.Models
{
    internal class AssetsRoot
    {
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
