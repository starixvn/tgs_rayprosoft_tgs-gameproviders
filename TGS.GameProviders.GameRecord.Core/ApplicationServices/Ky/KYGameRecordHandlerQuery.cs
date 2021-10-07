using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.Ky
{
    public record KYGameRecordHandlerQuery
    {
        [JsonPropertyName("brandCode")]
        public string BrandCode { get; init; }
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; init; }
        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; init; }
        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; init; }
    }
}
