using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    public abstract record AsiaGamingBaseRequest
    {
        [JsonPropertyName("cagent")]
        public string AgentCode { get; init; }

        [JsonPropertyName("loginname")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("actype")]
        public int AcType => 1;

        [JsonPropertyName("password")]
        public string Password => "password";

        [JsonPropertyName("oddtype")]
        public string BetLimitId { get; init; } = string.Empty;

        [JsonPropertyName("cur")]
        public string Currency { get; init; }
    }
}