using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.BlackJack;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
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
