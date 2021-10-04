using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record InstantRoulette
    {
        [JsonPropertyName("outcomes")]
        public InstantRouletteOutcome[] Outcomes { get; set; }
    }
}
