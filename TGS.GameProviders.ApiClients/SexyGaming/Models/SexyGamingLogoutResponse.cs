using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.SexyGaming.Models.Commons;

namespace TGS.GameProviders.ApiClients.SexyGaming.Models
{
    public record SexyGamingLogoutResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("logoutUsers")]
        public string[] LogoutUsers { get; init; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
    }
}
