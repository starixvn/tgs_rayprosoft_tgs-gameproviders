using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Craps;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Craps
    {
        [JsonPropertyName("rolls")]
        public Roll[] Rolls { get; set; }
    }
}
