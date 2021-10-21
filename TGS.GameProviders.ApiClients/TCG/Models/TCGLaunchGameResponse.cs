using System.Text.Json.Serialization;
using TGS.GameProviders.ApiClients.TCG.Models.Commons;

namespace TGS.GameProviders.ApiClients.TCG.Models
{
    public record TCGLaunchGameResponse : TCGBaseResponse
    {
        [JsonPropertyName("game_url")]
        public string GameUrl { get; init; }
        [JsonPropertyName("pt_username")]
        public string PTUsername { get; init; }
        [JsonPropertyName("pt_password")]
        public string PTPassword { get; init; }
    }
}
