using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Habanero.Models;

public record HabaneroLogoutResponse
{
    [JsonPropertyName("Success")]
    public bool Success { get; init; }

    [JsonPropertyName("Message")]
    public string ErrorMessage { get; init; }
}