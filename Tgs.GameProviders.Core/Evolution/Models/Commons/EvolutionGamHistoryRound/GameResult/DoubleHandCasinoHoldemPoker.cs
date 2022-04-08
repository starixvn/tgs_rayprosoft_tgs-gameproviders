using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record DoubleHandCasinoHoldemPoker
    {
        [JsonPropertyName("dealer")]
        public PokerDealer Dealer { get; set; }
        [JsonPropertyName("cards")]
        public PokerCards Cards { get; set; }
        [JsonPropertyName("firstHand")]
        public DoubleHandCasinoHoldemPokerHand FirstHand { get; set; }
        [JsonPropertyName("secondHand")]
        public DoubleHandCasinoHoldemPokerHand SecondHand { get; set; }
    }
}
