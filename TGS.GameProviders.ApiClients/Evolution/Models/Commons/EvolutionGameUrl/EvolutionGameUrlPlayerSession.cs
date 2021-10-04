using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlPlayerSession
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
