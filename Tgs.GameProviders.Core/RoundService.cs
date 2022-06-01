using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRounds.Client;

namespace Tgs.GameProviders.Core
{
    internal class RoundService : IRoundService
    {
        private readonly IGameRoundsClient _client;

        public RoundService(IGameRoundsClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<RoundInfo> GetAsync(Guid roundId)
        {
            var response = await _client.GetGameRoundAsync(roundId).ConfigureAwait(false);

            if (!response.Success)
            {
                return null;
            }

            return new RoundInfo(
                    response.Data.RoundId,
                    response.Data.ExternalRoundId,
                    response.Data.GameProviderId,
                    response.Data.PlayerCode,
                    response.Data.GameId,
                    response.Data.CurrencyCode,
                    response.Data.Status,
                    response.Data.GameProviderConfigurationId,
                    response.Data.WalletId,
                    response.Data.CreatedOn,
                    response.Data.ClosedOn,
                    response.Data.UpdatedOn,
                    response.Data.BrandId,
                    response.Data.ExternalCreatedOn,
                    response.Data.ExternalClosedOn,
                    response.Data.BetAmount,
                    response.Data.ValidBetAmount,
                    response.Data.SettleAmount,
                    response.Data.Commission,
                    response.Data.Turnover,
                    response.Data.CompanyWinLoss,
                    response.Data.TieBetAdjustment,
                    response.Data.ClientStatus,
                    response.Data.ReopenTurnoverAdjustment,
                    response.Data.ReopenedOn,
                    response.Data.EffectiveBettingPercentage,
                    response.Data.AdditionalData
                     );
        }
    }
}
