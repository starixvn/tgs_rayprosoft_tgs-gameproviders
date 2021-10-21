// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record BaseRequest
    {
        [JsonPropertyName("CompanyKey")]
        public string CompanyKey { get; init; }

        [JsonPropertyName("ServerId")]
        public string ServerId { get; init; } = "sbo-api";
    }
}