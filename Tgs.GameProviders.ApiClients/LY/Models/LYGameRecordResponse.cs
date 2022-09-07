using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.LY.Models.Commons;
using Tgs.GameProviders.Core.LY.Models;

namespace Tgs.GameProviders.ApiClients.LY.Models;

public record LYGameRecordResponse : LYBaseDataResponse
{
    [JsonPropertyName("start")]
    public long Start { get; init; }
    [JsonPropertyName("end")]
    public long End { get; init; }
    [JsonPropertyName("count")]
    public int Count { get; init; }
    [JsonPropertyName("list")]
    public LYGameRecordList List { get; init; }
}
