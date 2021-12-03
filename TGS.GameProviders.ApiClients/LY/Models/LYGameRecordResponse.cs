using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.LY.Models.Commons;
using TGS.GameProviders.Core.LY.Models;

namespace TGS.GameProviders.ApiClients.LY.Models
{
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
}
