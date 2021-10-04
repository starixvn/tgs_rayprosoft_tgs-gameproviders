using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SexyGaming.Models.Commons
{
    public record SexyGamingBaseRequest
    {
        [JsonPropertyName("cert")]
        public string Cert { get; set; }
        [JsonPropertyName("agentId")]
        public string AgentId { get; set; }
    }
}
