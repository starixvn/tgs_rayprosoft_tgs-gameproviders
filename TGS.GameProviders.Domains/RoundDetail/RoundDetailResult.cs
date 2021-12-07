using System.Text.Json.Serialization;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailResult
    {
        [JsonPropertyName("gpname")]
        public string GameProviderName { get; init; } = default!;

        [JsonPropertyName("gname")]
        public string GameName { get; init; } = default!;

        [JsonPropertyName("externalroundid")]
        public string ExternalRoundId { get; init; } = default!;

        [JsonPropertyName("betamount")]
        public decimal? BetAmount { get; init; }

        [JsonPropertyName("turnover")]
        public decimal? Turnover { get; init; }

        [JsonPropertyName("winloss")]
        public decimal? Winloss { get; init; }

        [JsonPropertyName("result")]
        public string GameResult { get; init; } = default!;

        [JsonPropertyName("data")]
        public RoundDetailDataBase[]? Data { get; init; }
    }
}
