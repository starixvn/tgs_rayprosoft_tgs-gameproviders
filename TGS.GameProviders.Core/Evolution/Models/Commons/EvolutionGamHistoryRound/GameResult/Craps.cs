using System.Text.Json.Serialization;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Craps;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record Craps
    {
        [JsonPropertyName("rolls")]
        public Roll[] Rolls { get; set; }
    }
}
