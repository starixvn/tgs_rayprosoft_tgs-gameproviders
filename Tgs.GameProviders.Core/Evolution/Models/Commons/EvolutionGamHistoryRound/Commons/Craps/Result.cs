using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.Craps
{
    public record Result
    {
        [JsonPropertyName("first")]
        public int First { get; set; }
        [JsonPropertyName("second")]
        public int Second { get; set; }
    }
}
