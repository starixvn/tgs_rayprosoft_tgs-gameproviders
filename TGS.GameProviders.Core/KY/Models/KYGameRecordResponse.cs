using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYGameRecordResponse : KYBaseDataResponse
    {
        [JsonPropertyName("start")]
        public long Start { get; init; }
        [JsonPropertyName("end")]
        public long End { get; init; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
        [JsonPropertyName("list")]
        public KYGameRecordList List { get; init; }
    }
}
