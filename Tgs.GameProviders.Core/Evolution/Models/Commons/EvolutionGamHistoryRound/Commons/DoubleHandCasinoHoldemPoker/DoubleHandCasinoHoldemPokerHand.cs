using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.DoubleHandCasinoHoldemPoker;

public record DoubleHandCasinoHoldemPokerHand
{
    [JsonPropertyName("player")]
    public PokerPlayer Player { get; set; }
    [JsonPropertyName("outcome")]
    public string Outcome { get; set; }
}
