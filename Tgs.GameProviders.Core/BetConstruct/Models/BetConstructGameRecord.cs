using System.Text.Json.Serialization;

#nullable enable

namespace Tgs.GameProviders.Core.BetConstruct.Models
{
    public record BetConstructGameRecord
    {
        [JsonPropertyName("SelectionId")]
        public long? SelectionId { get; init; }

        [JsonPropertyName("State")]
        public BetSelectionState? State { get; init; }

        public BetConstructBetPlacedSelection[]? Bet { get; init; }

        public BetConstructBetResultedSelection[]? Settle { get; init; }
    }
}