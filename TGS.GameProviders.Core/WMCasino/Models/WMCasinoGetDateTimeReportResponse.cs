using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoGetDateTimeReportResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public WMCasinoBetResult[] Results { get; init; } = default!;
    }
}
