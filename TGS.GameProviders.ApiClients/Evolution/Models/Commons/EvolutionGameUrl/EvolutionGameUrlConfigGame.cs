using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigGame
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("interface")]
        public string @Interface { get; set; }
        [JsonPropertyName("table")]
        public EvolutionGameUrlConfigGameTable Table { get; set; }
    }
}
