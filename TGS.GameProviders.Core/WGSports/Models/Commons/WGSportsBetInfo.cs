using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WGSports.Models.Commons
{
    public record WGSportsBetInfo
    {
        [JsonPropertyName("member_id")]
        public string MemberId { get; init; }
        [JsonPropertyName("username")]
        public string Username { get; init; }
        [JsonPropertyName("record_date")]
        public string RecordDate { get; init; }
        [JsonPropertyName("reference")]
        public string Reference { get; init; }
        [JsonPropertyName("sport_type")]
        public string SportType { get; init; }
        [JsonPropertyName("bet_type")]
        public string BetType { get; init; }
        [JsonPropertyName("match_name")]
        public string MatchName { get; init; }
        [JsonPropertyName("home_name")]
        public string HomeName { get; init; }
        [JsonPropertyName("away_name")]
        public string AwayName { get; init; }
        [JsonPropertyName("bet_content")]
        public string BetContent { get; init; }
        [JsonPropertyName("bet_amount")]
        public string BetAmount { get; init; }
        [JsonPropertyName("bet_status")]
        public string BetStatus { get; init; }
        [JsonPropertyName("result")]
        public string Result { get; init; }
        [JsonPropertyName("active_amount")]
        public string ActiveAmount { get; init; }
        [JsonPropertyName("result_score")]
        public string ResultScore { get; init; }
        [JsonPropertyName("settle_date")]
        public string SettleDate { get; init; }
        [JsonPropertyName("bet_ip")]
        public string BetIp { get; init; }
        [JsonPropertyName("init_data")]
        public WGSportsInitData InitData { get; init; }
    }
}
