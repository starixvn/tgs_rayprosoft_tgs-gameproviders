using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AllBet.Models;

public record AllBetCheckOrCreateMemberResponse
{
    [JsonPropertyName("resultCode")]
    public string ErrorCode { get; init; } = default!;

    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; } = default!;

    [JsonPropertyName("player")]
    public AllBetCheckOrCreateMemberData Data { get; init; }
}
