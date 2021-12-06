// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.SBO.Models
{
    public record SBOBasePlayerRequest : SBOBaseRequest
    {
        [JsonPropertyName("Username")]
        public string PlayerCode { get; init; }
    }
}