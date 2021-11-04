using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlPlayerGroup
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
