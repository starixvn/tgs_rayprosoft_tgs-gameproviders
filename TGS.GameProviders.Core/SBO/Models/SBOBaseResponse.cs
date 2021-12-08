// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.SBO.Models
{
    public record SBOBaseResponse
    {
        [JsonPropertyName("error")]
        public SBOErrorData Error { get; init; }

        [JsonPropertyName("serverId")]
        public string ServerId { get; init; }
    }
}