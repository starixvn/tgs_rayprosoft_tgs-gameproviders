using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl;

namespace TGS.GameProviders.ApiClients.Evolution.Models
{
    public record EvolutionGameUrlRequest
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        [JsonPropertyName("player")]
        public EvolutionGameUrlPlayer Player { get; set; }
        [JsonPropertyName("config")]
        public EvolutionGameUrlConfig Config { get; set; }
    }
}
