using System;

namespace TGS.GameProviders.Repositories.Domains.Models.Rounds
{
    public record UpdateRoundByExternalRoundId(
        string ExternalRoundId,
        Guid GameProviderId,
        int GameId,
        RoundStatus Status,
        DateTimeOffset? ClosedOn,
        DateTimeOffset UpdatedOn,
        DateTimeOffset? ExternalCreatedOn,
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
        string AdditionalData
        );
}
