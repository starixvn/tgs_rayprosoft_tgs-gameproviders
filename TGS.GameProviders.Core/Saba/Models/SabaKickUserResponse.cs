using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Saba.Models
{
    public record SabaKickUserResponse
    {
        [JsonPropertyName("error_code")]
        public SabaKickUserApiError ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }
    }
}