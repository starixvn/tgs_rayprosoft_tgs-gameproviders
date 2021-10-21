// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.SBO.Models
{
    public record LoginRequest : BasePlayerRequest
    {
        [JsonPropertyName("Portfolio")]
        public string Portfolio { get; init; }
    }
}