using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AllBet.Models;

public record AllBetCheckOrCreateMemberData
{
    [JsonPropertyName("player")]
    public string PlayerId { get; init; }
}
