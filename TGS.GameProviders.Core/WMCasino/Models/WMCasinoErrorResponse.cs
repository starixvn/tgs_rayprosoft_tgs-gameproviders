// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoErrorResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; } = default!;
    }
}
