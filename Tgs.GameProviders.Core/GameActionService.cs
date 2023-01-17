using System;
using System.Collections.Generic;
using Tgs.GameCommand.Client;

namespace Tgs.GameProviders.Core;

internal class GameActionService : IGameActionService
{
    private readonly IGameCommandClient _client;

    public GameActionService(
        IGameCommandClient client
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameActionInfo> QueryAsync(Guid roundId)
    {
        var viewModels = await _client.GameActionQueryAsync(roundId).ConfigureAwait(false);

        foreach (var viewModel in viewModels)
            yield return new GameActionInfo(
                viewModel.Id,
                viewModel.RoundId,
                viewModel.PlayerCode,
                viewModel.ExternalTransactionId,
                viewModel.Type,
                viewModel.Amount,
                viewModel.Turnover,
                viewModel.WinLoss,
                viewModel.UnsettledBets,
                viewModel.Balance,
                viewModel.BetType,
                viewModel.ExternalBetId,
                viewModel.ExternalGameActionGroupId,
                viewModel.ReferencedGameActionId,
                viewModel.GameActionBatchId,
                viewModel.RecordedOn,
                viewModel.SentOn,
                viewModel.IsClosingRound,
                viewModel.ClosingRoundExplicitRequest,
                viewModel.PlayerIp,
                viewModel.ExtraData,
                viewModel.JackpotContribution,
                viewModel.StoredOn);
    }
}
