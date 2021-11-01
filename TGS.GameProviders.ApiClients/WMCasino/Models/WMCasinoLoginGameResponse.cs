// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WMCasino.Models
{
    public record WMCasinoLoginGameResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public string Url { get; set; }
    }
}