using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Jili.Models;

public record JiliLoginWithoutRedirectRequest
{
    [JsonPropertyName("Token")]
    public string Token { get; init; }

    [JsonPropertyName("GameId")]
    public string GameExternalId { get; init; }

    [JsonPropertyName("Lang")]
    public string Language { get; init; }
}
