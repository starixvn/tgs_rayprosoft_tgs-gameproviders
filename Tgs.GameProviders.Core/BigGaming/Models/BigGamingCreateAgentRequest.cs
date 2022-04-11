using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;

namespace Tgs.GameProviders.Core.BigGaming.Models
{
    public record BigGamingCreateAgentRequest : BigGamingParamsSignRequest
    {
        [JsonPropertyName("loginId")]
        public string LoginId { get; init; } = default!;
        [JsonPropertyName("password")]
        public string Password { get; init; } = default!;
        [JsonPropertyName("fromIp")]
        public string FromIp { get; init; } = default!;
    }
}
