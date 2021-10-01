using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.WGSports.Models.Commons;

namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsGameRecordResponse
    {
        [JsonPropertyName("bet_info")]
        public WGSportsBetInfo[] BetInfo { get; set; }
        [JsonPropertyName("total")]
        public string Total { get; set; }
        [JsonPropertyName("total_amount")]
        public string TotalAmount { get; set; }
        [JsonPropertyName("total_result")]
        public string TotalResult { get; set; }
        [JsonPropertyName("total_active_amount")]
        public string TotalActiveAmount { get; set; }
        [JsonPropertyName("ReturnCode")]
        public string ReturnCode { get; set; }
    }
}
