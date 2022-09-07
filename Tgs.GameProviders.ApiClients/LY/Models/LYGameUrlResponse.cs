using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.LY.Models.Commons;

namespace Tgs.GameProviders.ApiClients.LY.Models;

public record LYGameUrlResponse : LYBaseDataResponse
{
    [JsonPropertyName("url")]
    public string Url { get; init; }
}
