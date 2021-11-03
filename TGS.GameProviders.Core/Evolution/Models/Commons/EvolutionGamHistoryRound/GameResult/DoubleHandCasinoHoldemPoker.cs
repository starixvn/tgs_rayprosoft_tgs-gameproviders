using System.Text.Json.Serialization;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
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
