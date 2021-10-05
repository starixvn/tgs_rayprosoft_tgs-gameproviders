using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
