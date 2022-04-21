using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.PragmaticPlay.Models
{
    public record PragmaticPlayOpenHistoryExtendedRequest
    {
        [JsonPropertyName("language")]
        public string Language { get; init; }

        [JsonPropertyName("playerId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("roundId")]
        public string RoundId { get; init; }

        [JsonPropertyName("secureLogin")]
        public string SecureLogin { get; init; }
    }
}
