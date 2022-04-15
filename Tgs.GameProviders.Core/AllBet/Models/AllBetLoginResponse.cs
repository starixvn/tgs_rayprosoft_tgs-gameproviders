using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AllBet.Models;

public record AllBetLoginResponse
{
    [JsonPropertyName("resultCode")]
    public string ErrorCode { get; init; } = default!;

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; } = default!;

    [JsonPropertyName("data")]
    public AllBetLoginData Data { get; init; }
}
