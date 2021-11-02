using System;
using TGS.GameProviders.Repositories.Domains;

namespace TGS.GameProviders.Client
{
    public record GameRecordListViewModle
    {
        public string ExternalRoundId { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public string ExternalGameId { get; init; } = default!;
        public RoundStatus Status { get; init; }
        public DateTimeOffset? ExternalCreatedOn { get; init; }
        public DateTimeOffset? ExternalClosedOn { get; init; }
        public decimal? BetAmount { get; init; }
        public decimal? ValidBetAmount { get; init; }
        public decimal? SettleAmount { get; init; }
        public decimal? Commission { get; init; }
        public decimal? Turnover { get; init; }
        public decimal? PlayerWinLoss { get; init; }
        public decimal? TieBetAdjustment { get; init; }
        public ClientRoundStatus ClientStatus { get; init; }
        public string AdditionalData { get; init; } = default!;
    }
}
