using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingCreatePlayerResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }
        [JsonPropertyName("userId")]
        public long UserId { get; init; }
        [JsonPropertyName("nickname")]
        public string Nickname { get; init; }
        [JsonPropertyName("regType")]
        public string RegType { get; init; }
    }
}
