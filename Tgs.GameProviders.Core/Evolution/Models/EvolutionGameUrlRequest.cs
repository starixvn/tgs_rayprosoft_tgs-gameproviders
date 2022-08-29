using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl;

namespace Tgs.GameProviders.Core.Evolution.Models;

public record EvolutionGameUrlRequest
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; }
    [JsonPropertyName("player")]
    public EvolutionGameUrlPlayer Player { get; set; }
    [JsonPropertyName("config")]
    public EvolutionGameUrlConfig Config { get; set; }
}
