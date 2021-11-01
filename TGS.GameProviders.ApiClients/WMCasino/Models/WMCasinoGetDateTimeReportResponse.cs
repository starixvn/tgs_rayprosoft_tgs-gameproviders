using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WMCasino.Models
{
    public record WMCasinoGetDateTimeReportResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public WMCasinoBetResult[] Results { get; init; }
    }
}