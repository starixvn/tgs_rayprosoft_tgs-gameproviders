using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.IMESports.Models
{
    public record IMESportsHistoryResult
    {
        [JsonPropertyName("Provider")]
        public string Provider { get; init; }

        [JsonPropertyName("GameId")]
        public string GameId { get; init; }

        [JsonPropertyName("BetId")]
        public string BetId { get; init; }

        [JsonPropertyName("PlayerId")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("StakeAmount")]
        public decimal StakeAmount { get; init; }

        [JsonPropertyName("WinLoss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("ProviderPlayerId")]
        public string ProviderPlayerId { get; init; }

        [JsonPropertyName("WagerType")]
        public string WagerType { get; init; }

        [JsonPropertyName("IsSettled")]
        public string StrIsSettle { get; init; }

        [JsonIgnore]
        public bool IsSettled => StrIsSettle == "1";

        [JsonPropertyName("DetailItems")]
        public IMESportsHistoryDetail[] Details { get; init; }
    }
}