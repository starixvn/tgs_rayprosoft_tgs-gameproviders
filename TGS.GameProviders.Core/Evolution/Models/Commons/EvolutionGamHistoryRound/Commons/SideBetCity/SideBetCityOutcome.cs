using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound.Commons.SideBetCity
{
    public record SideBetCityOutcome
    {
        [JsonPropertyName("threeCards")]
        public SideBetCityCards ThreeCards { get; set; }
        [JsonPropertyName("fiveCards")]
        public SideBetCityCards FiveCards { get; set; }
        [JsonPropertyName("sevenCards")]
        public SideBetCityCards SevenCards { get; set; }
    }
}
