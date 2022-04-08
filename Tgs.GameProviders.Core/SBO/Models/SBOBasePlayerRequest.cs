// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models
{
    public record SBOBasePlayerRequest : SBOBaseRequest
    {
        [JsonPropertyName("Username")]
        public string PlayerCode { get; init; }
    }
}
