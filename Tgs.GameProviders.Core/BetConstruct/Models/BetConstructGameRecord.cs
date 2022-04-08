using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BetConstruct.Models
{
    public record BetConstructGameRecord
    {
        [JsonPropertyName("SelectionId")]
        public long SelectionId { get; init; }

        [JsonPropertyName("State")]
        public BetSelectionState State { get; init; }
    }
}
