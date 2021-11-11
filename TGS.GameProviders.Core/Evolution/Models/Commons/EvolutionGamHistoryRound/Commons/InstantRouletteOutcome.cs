using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons
{
    public record InstantRouletteOutcome
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }
        [JsonPropertyName("wheel")]
        public string Wheel { get; set; }
        [JsonPropertyName("spin")]
        public string Spin { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}
