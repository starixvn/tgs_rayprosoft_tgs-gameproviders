using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.VR
{
    public record VRGameRecordHandlerQuery
    {
        [JsonPropertyName("brandCode")]
        public string BrandCode { get; init; }
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}
