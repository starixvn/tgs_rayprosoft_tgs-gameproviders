using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.SexyGaming.Models.Commons;

namespace Tgs.GameProviders.Core.SexyGaming.Models
{
    public record SexyGamingLogoutResponse : SexyGamingBaseResponse
    {
        [JsonPropertyName("logoutUsers")]
        public string[] LogoutUsers { get; init; }
        [JsonPropertyName("count")]
        public int Count { get; init; }
    }
}
