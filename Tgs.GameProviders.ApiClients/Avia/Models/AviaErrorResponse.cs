using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.Avia.Models
{
    internal record AviaErrorResponse
    {
        [JsonPropertyName("Error")]
        public string Error { get; init; }
    }
}
