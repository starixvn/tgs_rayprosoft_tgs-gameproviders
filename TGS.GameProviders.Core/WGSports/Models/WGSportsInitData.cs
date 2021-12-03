using System.Text.Json.Serialization;

namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsInitData
    {
        [JsonPropertyName("FB")]
        public string FB { get; init; } = default!;
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
        [JsonPropertyName("content")]
        public string Content { get; init; } = default!;
        [JsonPropertyName("valid")]
        public string Valid { get; init; } = default!;
        [JsonPropertyName("live_status")]
        public string LiveStatus { get; init; } = default!;
        [JsonPropertyName("plan_no")]
        public string PlanNo { get; init; } = default!;
        [JsonPropertyName("part")]
        public string Part { get; init; } = default!;
        [JsonPropertyName("win")]
        public string Win { get; init; } = default!;
        [JsonPropertyName("odds_typewin")]
        public string OddsType { get; init; } = default!;
        [JsonPropertyName("match_date")]
        public string MatchDate { get; init; } = default!;
        [JsonPropertyName("amount")]
        public string BetAmount { get; init; } = default!;
    }
}
