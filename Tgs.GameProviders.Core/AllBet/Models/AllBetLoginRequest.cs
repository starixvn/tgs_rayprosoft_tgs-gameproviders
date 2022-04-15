using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AllBet.Models;

public record AllBetLoginRequest
{
    [JsonPropertyName("player")]
    public string PlayerId { get; init; }

    [JsonPropertyName("language")]
    public string Language { get; init; }
}
