// ReSharper disable InconsistentNaming

using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoGetDateTimeReportRequest : WMCasinoBaseRequest
    {
        public override string Action => "GetDateTimeReport";

        [JsonIgnore]
        public DateTimeOffset? EndTime { get; init; }

        [JsonPropertyName("endTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EndTimeString => EndTime?.ToString("yyyyMMddHHmmss");

        [JsonPropertyName("syslang")]
        public string Language => "1"; // 0:中文, 1:英文

        [JsonPropertyName("timetype")]
        public string ReportTimeType => "1"; // 0:下注时间, 1:结算时间

        [JsonPropertyName("datatype")]
        public string ReportType => "0"; // 0:输赢报表, 1:小费报表, 2:全部

        [JsonIgnore]
        public DateTimeOffset StartTime { get; init; }

        [JsonPropertyName("startTime")]
        public string StartTimeString => StartTime.ToString("yyyyMMddHHmmss");
    }
}
