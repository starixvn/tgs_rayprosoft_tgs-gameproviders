using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.SideBetCity;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record SideBetCity
    {
        [JsonPropertyName("cardsDealt")]
        public string[] CardsDealt { get; set; }
        [JsonPropertyName("outcome")]
        public SideBetCityOutcome Outcome { get; set; }
    }
}
