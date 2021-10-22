// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record BaseResponse
    {
        [JsonPropertyName("error")]
        public ErrorData Error { get; init; }

        [JsonPropertyName("serverId")]
        public string ServerId { get; init; }
    }
}