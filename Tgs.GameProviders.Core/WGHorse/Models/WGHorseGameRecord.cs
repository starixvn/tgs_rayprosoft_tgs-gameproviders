using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WGHorse.Models
{
    public record WGHorseGameRecord
    {
        [JsonPropertyName("memberCode")]
        public string MemberCode { get; init; } = default!;
        [JsonPropertyName("recordDate")]
        public string RecordDate { get; init; } = default!;
        [JsonPropertyName("batchNo")]
        public string BatchNo { get; init; } = default!;
        [JsonPropertyName("showType")]
        public string ShowType { get; init; } = default!;
        [JsonPropertyName("showDiscount")]
        public string ShowDiscount { get; init; } = default!;
        [JsonPropertyName("showContent")]
        public string ShowContent { get; init; } = default!;
        [JsonPropertyName("showResult")]
        public string ShowResult { get; init; } = default!;
        [JsonPropertyName("showAmount")]
        public string ShowAmount { get; init; } = default!;
        [JsonPropertyName("showWin")]
        public string ShowWin { get; init; } = default!;
        [JsonPropertyName("showOddsWin")]
        public string ShowOddsWin { get; init; } = default!;
        [JsonPropertyName("showDivideDate")]
        public string ShowDivideDate { get; init; } = default!;
        [JsonPropertyName("showRaceDate")]
        public string ShowRaceDate { get; init; } = default!;
        [JsonPropertyName("ArrayInvoiceChild")]
        public object[]? ArrayInvoiceChild { get; init; }
    }
}
