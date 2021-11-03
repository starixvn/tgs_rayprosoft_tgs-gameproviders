using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.BigGaming.Models
{
    public record BigGamingGameRecordFishingResponse
    {
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("gameType")]
        public int GameType { get; set; }
        [JsonPropertyName("issueId")]
        public string ExternalRoundId { get; set; }
        [JsonPropertyName("betId")]
        public string ExternalBetId { get; set; }
        [JsonPropertyName("fireCount")]
        public int FireCount { get; set; }
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; set; }
        [JsonPropertyName("validAmount")]
        public decimal ValidAmount { get; set; }
        [JsonPropertyName("calcAmount")]
        public decimal CalcAmount { get; set; }
        [JsonPropertyName("payout")]
        public decimal Payout { get; set; }
        [JsonPropertyName("orderTime")]
        public string OrderTime { get; set; }
        [JsonPropertyName("jackpot")]
        public string Jackpot { get; set; }
        [JsonPropertyName("jackpotType")]
        public int JackpotType { get; set; }
        [JsonPropertyName("extend")]
        public decimal Extend { get; set; }
        [JsonPropertyName("orderFrom")]
        public string OrderFrom { get; set; }
        [JsonPropertyName("gameBalance")]
        public string GameBalance { get; set; }
    }
}
