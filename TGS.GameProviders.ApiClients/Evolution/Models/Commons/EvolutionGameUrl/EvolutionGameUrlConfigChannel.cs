using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigChannel
    {
        [JsonPropertyName("wrapped")]
        public bool Wrapped { get; set; }
        [JsonPropertyName("mobile")]
        public bool Mobile { get; set; }
    }
}
