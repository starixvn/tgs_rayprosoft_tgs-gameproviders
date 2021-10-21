// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record LoginResponse : BaseResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; init; }
    }
}