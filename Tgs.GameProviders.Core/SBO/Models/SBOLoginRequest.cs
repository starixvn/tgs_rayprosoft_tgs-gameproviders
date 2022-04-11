// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models
{
    public record SBOLoginRequest : SBOBasePlayerRequest
    {
        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }
    }
}
