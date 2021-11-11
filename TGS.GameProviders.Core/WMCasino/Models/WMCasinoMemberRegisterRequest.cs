// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoMemberRegisterRequest : WMCasinoBaseRequest
    {
        public override string Action => "MemberRegister";

        [JsonPropertyName("password")]
        public string Password => "TgsP@ssword";

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; init; }
    }
}
