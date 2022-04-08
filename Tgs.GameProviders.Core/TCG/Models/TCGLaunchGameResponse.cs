using System.Text.Json.Serialization;
using Tgs.GameProviders.Core.TCG.Models.Commons;

namespace Tgs.GameProviders.Core.TCG.Models
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
