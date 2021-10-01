using System;

namespace TGS.GameProviders.Repositories.Domains.Models.Rounds
{
    public record UpdateRound(
        Guid RoundId,
        RoundStatus Status,
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        DateTimeOffset? ExternalClosedOn,
        decimal? BetAmount,
        decimal? ValidBetAmount,
        decimal? SettleAmount,
        decimal? Commission,
        decimal? Turnover,
        decimal? WinLoss,
        decimal? TieBetAdjustment,
        ClientRoundStatus ClientStatus,
        decimal? ReopenTurnoverAdjustment,
        DateTimeOffset? ReopenedOn,
        string AdditionalData);
}
