using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlConfigGameTable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
