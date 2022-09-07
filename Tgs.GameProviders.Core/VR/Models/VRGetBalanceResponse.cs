using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.VR.Models.Commons;

namespace Tgs.GameProviders.Core.VR.Models;

public record VRGetBalanceResponse
{
    [JsonPropertyName("playerName")]
    public string PlayerName { get; init; }
    [JsonPropertyName("balance")]
    public decimal Balance { get; init; }
    [JsonPropertyName("games")]
    public VRGetBalanceGamesResponse[] Games { get; init; }
}
