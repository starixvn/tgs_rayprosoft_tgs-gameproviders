using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.BlackJack;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record BlackJack
    {
        [JsonPropertyName("dealer")]
        public Dealer Dealer { get; set; }
        [JsonPropertyName("seats")]
        public Seats Seats { get; set; }
        [JsonPropertyName("burnedCards")]
        public object[] BurnedCards { get; set; }
    }
}
