// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace Tgs.GameProviders.Core.WMCasino.Models
{
    public record WMCasinoBetResult
    {
        [JsonPropertyName("bet")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal BetAmount { get; init; }

        [JsonPropertyName("betId")]
        public string BetId { get; init; } = default!;

        [JsonPropertyName("betTime")]
        public string BetTime { get; init; } = default!;

        [JsonPropertyName("betResult")]
        public string BetType { get; init; } = default!;

        [JsonPropertyName("betCode")]
        public string BetTypeCode { get; init; } = default!;

        [JsonPropertyName("betwalletid")]
        public string BetWalletIds { get; init; } = default!;

        [JsonPropertyName("commission")]
        public string Commission { get; init; } = default!;

        [JsonPropertyName("gid")]
        public string ExternalGameId { get; init; } = default!;

        [JsonPropertyName("gameResult")]
        public string GameResult { get; init; } = default!;

        [JsonPropertyName("reset")]
        public string Reset { get; init; } = default!;

        [JsonPropertyName("resultwalletid")]
        public string ResultWalletIds { get; init; } = default!;

        [JsonPropertyName("round")]
        public string RoundId { get; init; } = default!;

        [JsonPropertyName("settime")]
        public string SettleTime { get; init; } = default!;

        [JsonPropertyName("subround")]
        public string SubRoundId { get; init; } = default!;

        [JsonPropertyName("user")]
        public string User { get; init; } = default!;

        [JsonPropertyName("validbet")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal ValidBet { get; init; }

        [JsonPropertyName("winLoss")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal WinLoss { get; init; }
    }
}
