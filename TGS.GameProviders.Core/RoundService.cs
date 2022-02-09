using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;

namespace TGS.GameProviders.Core
{
    internal class RoundService : IRoundService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public RoundService(
            IGameProvidersQueryServerClient gameProvidersQueryServerClient
            )
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<RoundInfo> GetAsync(Guid roundId)
        {
            var roundInfo = default(RoundInfo);
            var viewModel = await _gameProvidersQueryServerClient.RoundGetAsync(roundId).ConfigureAwait(false);

            if (viewModel != null)
            {
                roundInfo = new RoundInfo(
                    viewModel.RoundId,
                    viewModel.ExternalRoundId,
                    viewModel.GameProviderId,
                    viewModel.PlayerCode,
                    viewModel.GameId,
                    viewModel.CurrencyCode,
                    viewModel.Status,
                    viewModel.GameProviderConfigurationId,
                    viewModel.WalletId,
                    viewModel.CreatedOn,
                    viewModel.ClosedOn,
                    viewModel.UpdatedOn,
                    viewModel.BrandId,
                    viewModel.ExternalCreatedOn,
                    viewModel.ExternalClosedOn,
                    viewModel.BetAmount,
                    viewModel.ValidBetAmount,
                    viewModel.SettleAmount,
                    viewModel.Commission,
                    viewModel.Turnover,
                    viewModel.WinLoss,
                    viewModel.TieBetAdjustment,
                    viewModel.ClientStatus,
                    viewModel.ReopenTurnoverAdjustment,
                    viewModel.ReopenedOn,
                    viewModel.EffectiveBettingPercentage,
                    viewModel.AdditionalData
                     );
            }
            return roundInfo;
        }
    }
}
