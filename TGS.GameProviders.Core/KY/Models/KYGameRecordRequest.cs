using System.Text.Json.Serialization;
using TGS.GameProviders.Core.KY.Models.Commons;

namespace TGS.GameProviders.Core.KY.Models
{
    public record KYGameRecordRequest : KYBaseRequest
    {
        [JsonPropertyName("startTime")]
        public long StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public long EndTime { get; init; }
    }
}
