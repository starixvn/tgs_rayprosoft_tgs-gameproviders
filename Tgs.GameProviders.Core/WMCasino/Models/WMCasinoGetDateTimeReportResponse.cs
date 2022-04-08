using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoGetDateTimeReportResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public WMCasinoBetResult[] Results { get; init; } = default!;
    }
}
