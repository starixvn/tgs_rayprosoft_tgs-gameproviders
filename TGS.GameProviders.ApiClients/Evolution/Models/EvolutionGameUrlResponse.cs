using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons;

namespace TGS.GameProviders.ApiClients.Evolution.Models
{
    public record EvolutionGameUrlResponse
    {
        [JsonPropertyName("entry")]
        public string Entry { get; set; }
        [JsonPropertyName("entryEmbedded")]
        public string EntryEmbedded { get; set; }
        [JsonPropertyName("errors")]
        public EvolutionError[] Errors { get; set; }
    }
}
