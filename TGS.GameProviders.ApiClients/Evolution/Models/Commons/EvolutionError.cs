using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons
{
    public record EvolutionError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
