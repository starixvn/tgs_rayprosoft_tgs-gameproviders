using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.VR.Models;

public record VRCreatePlayerRequest
{
    [JsonPropertyName("playerName")]
    public string PlayerCode { get; init; }
}
