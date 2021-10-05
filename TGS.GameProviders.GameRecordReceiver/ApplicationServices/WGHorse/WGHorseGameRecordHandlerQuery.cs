using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGHorse
{
    public record WGHorseGameRecordHandlerQuery
    {
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}
