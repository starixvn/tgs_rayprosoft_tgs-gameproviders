using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record FrenchRoulette
    {
        [JsonPropertyName("outcomes")]
        public RouletteOutcomes[] Outcomes { get; set; }
    }
}
