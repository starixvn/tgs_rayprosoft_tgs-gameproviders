using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigChannel
    {
        [JsonPropertyName("wrapped")]
        public bool Wrapped { get; set; }
        [JsonPropertyName("mobile")]
        public bool Mobile { get; set; }
    }
}
