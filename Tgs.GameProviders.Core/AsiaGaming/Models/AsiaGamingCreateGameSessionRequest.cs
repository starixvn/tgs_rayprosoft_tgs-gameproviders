using System;
using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.AsiaGaming.Models
{
    public record AsiaGamingCreateGameSessionRequest
    {
        [JsonPropertyName("productid")]
        public string ProductId { get; init; } = default!;

        [JsonPropertyName("username")]
        public string PlayerCode { get; init; } = default!;

        [JsonPropertyName("session_token")]
        public Guid LoginRecordId { get; init; }

        [JsonPropertyName("credit")]
        public decimal Balance { get; init; }
    }
}
