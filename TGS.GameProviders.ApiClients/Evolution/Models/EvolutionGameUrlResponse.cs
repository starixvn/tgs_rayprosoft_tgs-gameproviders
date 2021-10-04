using System.Text.Json.Serialization;
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
