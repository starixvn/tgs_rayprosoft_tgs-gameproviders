using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.PragmaticPlay.Models
{
    public record PragmaticPlayLogoutRequest
    {
        [JsonPropertyName("externalPlayerId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("secureLogin")]
        public string SecureLogin { get; init; }
    }
}
