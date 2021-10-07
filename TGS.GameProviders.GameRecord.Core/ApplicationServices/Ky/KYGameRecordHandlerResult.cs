using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.Ky
{
    public record KYGameRecordHandlerResult
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
