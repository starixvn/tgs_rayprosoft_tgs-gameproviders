using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Evolution.Models.Commons.EvolutionGameUrl;

public record EvolutionGameUrlConfigGameTable
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
