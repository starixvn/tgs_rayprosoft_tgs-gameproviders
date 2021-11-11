// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoBetResult
    {
        [JsonPropertyName("bet")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("betId")]
        public string BetId { get; init; }

        [JsonPropertyName("betTime")]
        public string BetTime { get; init; }

        [JsonPropertyName("betResult")]
        public string BetType { get; init; }

        [JsonPropertyName("betCode")]
        public string BetTypeCode { get; init; }

        [JsonPropertyName("betwalletid")]
        public string BetWalletIds { get; init; }

        [JsonPropertyName("commission")]
        public string Commission { get; init; }

        [JsonPropertyName("gid")]
        public string ExternalGameId { get; init; }

        [JsonPropertyName("gameResult")]
        public string GameResult { get; init; }

        [JsonPropertyName("reset")]
        public string Reset { get; init; }

        [JsonPropertyName("resultwalletid")]
        public string ResultWalletIds { get; init; }

        [JsonPropertyName("round")]
        public string RoundId { get; init; }

        [JsonPropertyName("settime")]
        public string SettleTime { get; init; }

        [JsonPropertyName("subround")]
        public string SubRoundId { get; init; }

        [JsonPropertyName("user")]
        public string User { get; init; }

        [JsonPropertyName("validbet")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal ValidBet { get; init; }

        [JsonPropertyName("winLoss")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal WinLoss { get; init; }
    }
}
