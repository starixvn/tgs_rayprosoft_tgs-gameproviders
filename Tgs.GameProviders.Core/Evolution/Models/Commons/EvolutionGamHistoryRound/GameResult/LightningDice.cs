using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.GameResult
{
    public record LightningDice
    {
        [JsonPropertyName("first")]
        public int First { get; set; }
        [JsonPropertyName("second")]
        public int Second { get; set; }
        [JsonPropertyName("third")]
        public int Third { get; set; }
    }
}
