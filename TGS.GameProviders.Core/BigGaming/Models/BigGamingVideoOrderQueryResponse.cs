using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingVideoOrderQueryResponse
    {
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
        [JsonPropertyName("items")]
        public BigGamingVideoOrderQueryItemResponse[] Items { get; set; }
    }
}
