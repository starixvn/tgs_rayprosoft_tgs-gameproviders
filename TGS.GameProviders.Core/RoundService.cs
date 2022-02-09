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
            var data = await _gameProvidersQueryServerClient.RoundGetAsync(roundId).ConfigureAwait(false);

            if (data != null)
            {
                roundInfo = new RoundInfo(
                    data.RoundId,
                    data.ExternalRoundId,
                    data.GameProviderId,
                    data.PlayerCode,
                    data.GameId,
                    data.CurrencyCode,
                    data.Status,
                    data.GameProviderConfigurationId,
                    data.WalletId,
                    data.CreatedOn,
                    data.ClosedOn,
                    data.UpdatedOn,
                    data.BrandId,
                    data.ExternalCreatedOn,
                    data.ExternalClosedOn,
                    data.BetAmount,
                    data.ValidBetAmount,
                    data.SettleAmount,
                    data.Commission,
                    data.Turnover,
                    data.WinLoss,
                    data.TieBetAdjustment,
                    data.ClientStatus,
                    data.ReopenTurnoverAdjustment,
                    data.ReopenedOn,
                    data.EffectiveBettingPercentage,
                    data.AdditionalData
                     );
            }
            return roundInfo;
        }
    }
}
