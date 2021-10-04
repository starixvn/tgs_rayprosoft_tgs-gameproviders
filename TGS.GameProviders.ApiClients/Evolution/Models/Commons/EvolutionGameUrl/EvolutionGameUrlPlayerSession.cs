using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Evolution.Models.Commons.EvolutionGameUrl
{
    public record EvolutionGameUrlPlayerSession
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
    }
}
