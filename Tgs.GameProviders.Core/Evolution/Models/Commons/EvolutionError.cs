using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons;

public record EvolutionError
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
