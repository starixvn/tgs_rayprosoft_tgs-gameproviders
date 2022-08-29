using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.PragmaticPlay.Models;

public record PragmaticPlayGameUrlResponse : PragmaticPlayErrorResponse
{
    [JsonPropertyName("gameURL")]
    public string Url { get; init; }
}
