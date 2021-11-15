using System;

namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailResult
    {
        public decimal BetAmount { get; init; }
        public decimal Turnover { get; init; }
        public decimal Winloss { get; init; }
        public string GameResult { get; init; } = default!;

        public RoundDetailDataBase[] Data { get; init; } = Array.Empty<RoundDetailDataBase>();
    }
}
