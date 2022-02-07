using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.Jili.Models;

public record JiliGetGameDetailUrlData
{
    [JsonPropertyName("Url")]
    public string Url { get; init; }
}
