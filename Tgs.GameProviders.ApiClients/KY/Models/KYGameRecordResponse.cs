using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.KY.Models.Commons;
using Tgs.GameProviders.Core.KY.Models;

namespace Tgs.GameProviders.ApiClients.KY.Models
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
