using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.BigGaming
{
    public class BigGamingFishingGameRecordHandlerQuery
    {
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
    }
}
