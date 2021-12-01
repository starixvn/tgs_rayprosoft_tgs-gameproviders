using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Saba.Models
{
    public record SabaGameUrlResponse
    {
        [JsonPropertyName("error_code")]
        public SabaGetGameUrlApiError ErrorCode { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("Data")]
        public string Url { get; init; }
    }
}