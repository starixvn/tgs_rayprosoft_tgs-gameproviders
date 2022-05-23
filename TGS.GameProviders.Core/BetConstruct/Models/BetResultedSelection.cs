using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BetConstruct.Models
{
    public record BetResultedSelection
    {
        [JsonPropertyName("SelectionId")]
        public long SelectionId { get; set; }

        [JsonPropertyName("State")]
        public int State { get; set; }
    }
} 