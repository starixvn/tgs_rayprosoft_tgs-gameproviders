using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.Saba.Models;

public record SabaName
{
    [JsonPropertyName("lang")]
    public string Language { get; init; }

    [JsonPropertyName("name")]
    public string Value { get; init; }
}
