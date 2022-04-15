using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons;

namespace Tgs.GameProviders.Core.Evolution.Models
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
