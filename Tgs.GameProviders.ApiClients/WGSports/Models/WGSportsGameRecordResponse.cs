using System.Text.Json.Serialization;
using Tgs.GameProviders.ApiClients.WGSports.Models.Commons;
using Tgs.GameProviders.Core.WGSports.Models;

namespace Tgs.GameProviders.ApiClients.WGSports.Models;

public record WGSportsGameRecordResponse : WGSportsResponse
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
}
