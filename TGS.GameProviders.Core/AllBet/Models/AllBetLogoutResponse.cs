using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AllBet.Models;

public record AllBetLogoutResponse
{
    [JsonPropertyName("resultCode")]
    public string ErrorCode { get; init; } = default!;

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; } = default!;
}
