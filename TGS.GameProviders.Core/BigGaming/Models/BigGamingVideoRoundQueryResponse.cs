using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models;

public record BigGamingVideoRoundQueryResponse
{
    [JsonPropertyName("items")]
    public BigGamingVideoRoundQueryItemResponse[] Items { get; set; }
    [JsonPropertyName("total")]
    public int Total { get; set; }
    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    [JsonPropertyName("etag")]
    public string Etag { get; set; }
    [JsonPropertyName("page")]
    public int Page { get; set; }
}
