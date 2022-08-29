using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models;

public record VRKickRequest
{
    [JsonPropertyName("playerName")]
    public string PlayerCode { get; init; }
}
