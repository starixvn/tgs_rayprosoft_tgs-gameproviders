using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.WGSports
{
    public record WGSportsGameRecordHandlerQuery
    {
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}
