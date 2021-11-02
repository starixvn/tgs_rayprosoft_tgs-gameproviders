// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WMCasino.Models
{
    public record WMCasinoLogoutGameRequest : WMCasinoBaseRequest
    {
        public override string Action => "LogoutGame";

        [JsonPropertyName("user")]
        public string User { get; init; }
    }
}