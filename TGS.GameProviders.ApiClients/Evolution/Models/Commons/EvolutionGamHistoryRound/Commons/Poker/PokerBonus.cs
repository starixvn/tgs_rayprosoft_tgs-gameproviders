using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerBonus
    {
        [JsonPropertyName("tripsBonus")]
        public PokerBonusTripsbonus TripsBonus { get; set; }
    }
}
