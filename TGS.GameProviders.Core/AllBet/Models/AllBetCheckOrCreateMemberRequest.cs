using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AllBet.Models;

public record AllBetCheckOrCreateMemberRequest
{
    [JsonPropertyName("agent")]
    public string AgentId { get; init; }

    [JsonPropertyName("player")]
    public string PlayerId { get; init; }
}