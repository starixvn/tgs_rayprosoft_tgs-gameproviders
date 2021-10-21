// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record BasePlayerRequest : BaseRequest
    {
        [JsonPropertyName("Username")]
        public string PlayerCode { get; init; }
    }
}