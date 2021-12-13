using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.Avia.Models
{
    internal record AviaErrorResponse
    {
        [JsonPropertyName("Error")]
        public string Error { get; init; }
    }
}
