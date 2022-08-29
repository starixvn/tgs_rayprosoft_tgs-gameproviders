using System.Text.Json.Serialization;

namespace Tgs.GameProviders.ApiClients.Avia.Models;

internal class AviaBalanceResponse
{
    [JsonPropertyName("Currency")]
    public string Currency { get; init; }

    [JsonPropertyName("Error")]
    public string Error { get; init; }

    [JsonPropertyName("Money")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Money { get; init; }
}
