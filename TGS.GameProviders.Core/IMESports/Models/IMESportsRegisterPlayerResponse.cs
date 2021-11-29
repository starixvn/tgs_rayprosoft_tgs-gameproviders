// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsRegisterPlayerResponse
    {
        [JsonPropertyName("Code")]
        public IMESportsGameApiError ErrorCode { get; init; }

        [JsonPropertyName("Message")]
        public string Message { get; init; }
    }
}