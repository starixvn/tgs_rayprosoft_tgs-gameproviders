using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl;

public record EvolutionGameUrlConfig
{
    [JsonPropertyName("brand")]
    public EvolutionGameUrlConfigBrand Brand { get; set; }
    [JsonPropertyName("game")]
    public EvolutionGameUrlConfigGame Game { get; set; }
    [JsonPropertyName("channel")]
    public EvolutionGameUrlConfigChannel Channel { get; set; }
    [JsonPropertyName("urls")]
    public EvolutionGameUrlConfigUrls Urls { get; set; }
    [JsonPropertyName("freeGames")]
    public bool FreeGames { get; set; }
}
