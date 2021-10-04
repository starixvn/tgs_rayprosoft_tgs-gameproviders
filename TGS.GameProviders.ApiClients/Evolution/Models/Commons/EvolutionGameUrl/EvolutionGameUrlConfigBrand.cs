using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigBrand
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("skin")]
        public string Skin { get; set; }
    }
}
