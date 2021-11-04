using System.Text.Json.Serialization;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY.Models
{
    public record LYGameRecordRequest : LYBaseRequest
    {
        [JsonPropertyName("startTime")]
        public long StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public long EndTime { get; init; }
    }
}
