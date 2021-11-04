using System;
using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.AsiaGaming.Models
{
    public record AsiaGamingCreateGameSessionRequest
    {
        [JsonPropertyName("productid")]
        public string ProductId { get; init; }

        [JsonPropertyName("username")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("session_token")]
        public Guid LoginRecordId { get; init; }

        [JsonPropertyName("credit")]
        public decimal Balance { get; init; }
    }
}