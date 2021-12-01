#nullable enable

using System;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.Repositories.Models.GameAction
{
    public record GameActionModel(
        Guid GameActionId,
        Guid RoundId,
        string PlayerCode,
        string ExternalTransactionId,
        GameActionType Type,
        decimal Amount,
        decimal Turnover,
        decimal WinLoss,
        decimal UnsettledBets,
        string? BetType,
        string ExternalBetId,
        string ExternalGameActionGroupId,
        Guid? ReferencedGameActionId,
        Guid? GameActionBatchId,
        DateTime RecordedOn,
        DateTime? SentOn,
        string? ExtraData,
        string? JackpotContribution);
}