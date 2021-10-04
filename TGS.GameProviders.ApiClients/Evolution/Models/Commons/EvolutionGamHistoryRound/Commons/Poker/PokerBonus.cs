using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Poker
{
    public record PokerBonus
    {
        [JsonPropertyName("tripsBonus")]
        public PokerBonusTripsbonus TripsBonus { get; set; }
    }
}
