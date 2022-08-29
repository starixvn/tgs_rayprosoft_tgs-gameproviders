using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.SBO.Models;

public record SBOErrorData
{
    [JsonPropertyName("id")]
    public SBOApiErrorCode Code { get; init; }

    [JsonPropertyName("msg")]
    public string Message { get; init; }
}
