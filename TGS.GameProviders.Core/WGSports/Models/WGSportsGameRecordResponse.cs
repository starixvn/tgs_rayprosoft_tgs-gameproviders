using System.Text.Json.Serialization;
using TGS.GameProviders.Core.WGSports.Models.Commons;

namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsGameRecordResponse
    {
        [JsonPropertyName("bet_info")]
        public WGSportsBetInfo[]? BetInfo { get; set; }
        [JsonPropertyName("total")]
        public string Total { get; set; } = default!;
        [JsonPropertyName("total_amount")]
        public string TotalAmount { get; set; } = default!;
        [JsonPropertyName("total_result")]
        public string TotalResult { get; set; } = default!;
        [JsonPropertyName("total_active_amount")]
        public string TotalActiveAmount { get; set; } = default!;
        [JsonPropertyName("ReturnCode")]
        public string ReturnCode { get; set; } = default!;
    }
}
