using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming
{
    public class BigGamingFishingGameRecordHandlerResult
    {
        [JsonPropertyName("count")]
        public int Count { get; init; }

        [JsonPropertyName("failCount")]
        public int FailCount { get; set; }

        [JsonPropertyName("lastGameRecorTime")]
        public DateTimeOffset? LastGameRecorTime { get; init; }

        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }
    }
}
