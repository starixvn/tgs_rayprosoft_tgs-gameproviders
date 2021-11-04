using System.Text.Json.Serialization;
using TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record FrenchRoulette
    {
        [JsonPropertyName("outcomes")]
        public RouletteOutcomes[] Outcomes { get; set; }
    }
}
