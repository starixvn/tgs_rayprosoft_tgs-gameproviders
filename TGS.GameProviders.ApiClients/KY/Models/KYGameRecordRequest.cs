using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients.KY.Models
{
    public record KYGameRecordRequest : KYBaseRequest
    {
        [JsonPropertyName("startTime")]
        public long StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public long EndTime { get; init; }
    }
}
