using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WGSports.Models.Commons
{
    public record WGSportsBetInfo
    {
        [JsonPropertyName("member_id")]
        public string MemberId { get; init; } = default!;
        [JsonPropertyName("username")]
        public string Username { get; init; } = default!;
        [JsonPropertyName("record_date")]
        public string RecordDate { get; init; } = default!;
        [JsonPropertyName("reference")]
        public string Reference { get; init; } = default!;
        [JsonPropertyName("sport_type")]
        public string SportType { get; init; } = default!;
        [JsonPropertyName("bet_type")]
        public string BetType { get; init; } = default!;
        [JsonPropertyName("match_name")]
        public string MatchName { get; init; } = default!;
        [JsonPropertyName("home_name")]
        public string HomeName { get; init; } = default!;
        [JsonPropertyName("away_name")]
        public string AwayName { get; init; } = default!;
        [JsonPropertyName("bet_content")]
        public string BetContent { get; init; } = default!;
        [JsonPropertyName("bet_amount")]
        public string BetAmount { get; init; } = default!;
        [JsonPropertyName("bet_status")]
        public string BetStatus { get; init; } = default!;
        [JsonPropertyName("result")]
        public string Result { get; init; } = default!;
        [JsonPropertyName("active_amount")]
        public string ActiveAmount { get; init; } = default!;
        [JsonPropertyName("result_score")]
        public string ResultScore { get; init; } = default!;
        [JsonPropertyName("settle_date")]
        public string SettleDate { get; init; } = default!;
        [JsonPropertyName("bet_ip")]
        public string BetIp { get; init; } = default!;
        [JsonPropertyName("init_data")]
        public WGSportsInitData? InitData { get; init; }
    }
}
