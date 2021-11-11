using System.Text.Json.Serialization;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker
{
    public record DoubleHandCasinoHoldemPokerHand
    {
        [JsonPropertyName("player")]
        public PokerPlayer Player { get; set; }
        [JsonPropertyName("outcome")]
        public string Outcome { get; set; }
    }
}
