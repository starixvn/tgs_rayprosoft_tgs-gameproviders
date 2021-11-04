using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGamHistoryRound
{
    public record EvolutionGamHistoryRoundDataTable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
