// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.SBO.Models
{
    public record SBOLoginResponse : SBOBaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}
