using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.BigGaming.Models
{
    public record BigGamingCreateAgentRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; }
        [JsonPropertyName("password")]
        public string Password { get; init; }
        [JsonPropertyName("fromIp")]
        public string FromIp { get; init; }
    }
}
