using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Roulette
    {
        [JsonPropertyName("outcomes")]
        public RouletteOutcomes[] Outcomes { get; set; }
    }
}
