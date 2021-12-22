using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AllBet.Models;

public record AllBetLogoutRequest
{
    [JsonPropertyName("player")]
    public string PlayerId { get; init; }
}
