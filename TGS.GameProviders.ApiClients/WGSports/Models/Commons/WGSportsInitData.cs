using System.Text.Json.Serialization;

namespace TGS.GameProviders.ApiClients.WGSports.Models.Commons
{
    public record WGSportsInitData
    {
        [JsonPropertyName("FB")]
        public string FB { get; init; }
        [JsonPropertyName("type")]
        public string Type { get; init; }
        [JsonPropertyName("content")]
        public string Content { get; init; }
        [JsonPropertyName("valid")]
        public string Valid { get; init; }
        [JsonPropertyName("live_status")]
        public string LiveStatus { get; init; }
        [JsonPropertyName("plan_no")]
        public string PlanNo { get; init; }
        [JsonPropertyName("part")]
        public string Part { get; init; }
        [JsonPropertyName("win")]
        public string Win { get; init; }
        [JsonPropertyName("odds_typewin")]
        public string OddsType { get; init; }
        [JsonPropertyName("match_date")]
        public string MatchDate { get; init; }
        [JsonPropertyName("amount")]
        public string BetAmount { get; init; }
    }
}
