using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

public record PokerBonus
{
    [JsonPropertyName("tripsBonus")]
    public PokerBonusTripsbonus TripsBonus { get; set; }
}
