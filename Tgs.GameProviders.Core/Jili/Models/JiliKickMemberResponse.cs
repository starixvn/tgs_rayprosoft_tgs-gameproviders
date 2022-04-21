using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Jili.Models;

public record JiliKickMemberResponse
{
    [JsonPropertyName("ErrorCode")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("Message")]
    public string ErrorMessage { get; init; }
}
