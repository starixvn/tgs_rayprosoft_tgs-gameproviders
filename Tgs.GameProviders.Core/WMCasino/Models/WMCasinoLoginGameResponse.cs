// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoLoginGameResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public string Url { get; set; } = default!;
    }
}
