// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoLoginGameRequest : WMCasinoBaseRequest
    {
        public override string Action => "LoginGame";

        [JsonPropertyName("lang")]
        public string Language { get; init; } = default!;

        [JsonPropertyName("password")]
        public string Password => "TgsP@ssword";

        [JsonPropertyName("user")]
        public string User { get; set; } = default!;
    }
}
