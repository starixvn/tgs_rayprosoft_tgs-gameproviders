// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsLaunchGameResponse
    {
        [JsonPropertyName("Code")]
        public IMESportsGameApiError ErrorCode { get; init; }

        [JsonPropertyName("Message")]
        public string Message { get; init; } = default!;

        [JsonPropertyName("GameUrl")]
        public string GameUrl { get; init; } = default!;

        [JsonPropertyName("SessionToken")]
        public string SessionToken { get; init; } = default!;

        [JsonPropertyName("TTL")]
        public int TimeToLeave { get; init; }
    }
}