using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.GameRecordReceiver.ApplicationServices.WGHorse
{
    public record WGHorseGameRecordHandlerResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("lastGameRecorTime")]
        public DateTimeOffset? LastGameRecorTime { get; init; }
        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }
        [JsonPropertyName("failCount")]
        public int FailCount { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
    }
}
