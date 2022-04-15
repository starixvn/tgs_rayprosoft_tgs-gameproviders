using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.BigGaming.Models
{
    public record BigGamingCreatePlayerResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; } = default!;
        [JsonPropertyName("userId")]
        public long UserId { get; init; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; init; } = default!;
        [JsonPropertyName("regType")]
        public string RegType { get; init; } = default!;
    }
}
