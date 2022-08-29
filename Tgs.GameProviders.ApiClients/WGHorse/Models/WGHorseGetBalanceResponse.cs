using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.WGHorse.Models.Commons;

namespace Tgs.GameProviders.ApiClients.WGHorse.Models;

public record WGHorseGetBalanceResponse : WGHorseResponse
{
    [JsonPropertyName("Balance")]
    public string Balance { get; init; }
    [JsonPropertyName("Currency")]
    public string Currency { get; init; }
}
