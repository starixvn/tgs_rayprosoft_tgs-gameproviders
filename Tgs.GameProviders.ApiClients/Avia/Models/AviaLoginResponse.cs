using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.Avia.Models;

internal record AviaLoginResponse
{
    [JsonPropertyName("Error")]
    public string Error { get; init; }

    [JsonPropertyName("Url")]
    public string Url { get; init; }
}
