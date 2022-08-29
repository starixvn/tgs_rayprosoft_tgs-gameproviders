using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.PragmaticPlay.Models;

public record PragmaticPlayLogoutRequest
{
    [JsonPropertyName("externalPlayerId")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("secureLogin")]
    public string SecureLogin { get; init; }
}
