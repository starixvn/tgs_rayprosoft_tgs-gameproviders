// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoMemberRegisterResponse : WMCasinoErrorResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; init; } = default!;
    }
}
