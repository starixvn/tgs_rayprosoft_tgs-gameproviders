using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.VR
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
