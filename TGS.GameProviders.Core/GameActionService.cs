using System;
using System.Collections.Generic;
using Tgs.GameProviders.QueryServer.Client;

namespace TGS.GameProviders.Core
{
    internal class GameActionService : IGameActionService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public GameActionService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async IAsyncEnumerable<GameActionInfo> QueryAsync(Guid roundId)
        {
            var viewModels = await _gameProvidersQueryServerClient.GameActionQueryAsync(roundId).ConfigureAwait(false);

            foreach (var viewModel in viewModels)
                yield return new GameActionInfo(
                    viewModel.Id,
                    viewModel.RoundId,
                    viewModel.PlayerCode,
                    viewModel.ExternalTransactionId,
                    (GameActionType)viewModel.Type,
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
}
