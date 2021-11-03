using System.Text.Json.Serialization;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record InstantRoulette
    {
        [JsonPropertyName("outcomes")]
        public InstantRouletteOutcome[] Outcomes { get; set; }
    }
}
