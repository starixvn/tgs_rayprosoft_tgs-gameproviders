using Tgs.GameProviders.Core;
using Tgs.GameRounds.Client;

namespace Tgs.GameProviders.Repositories;

internal class RoundRepository : IRoundRepository
{
    private readonly IGameRoundsClient _client;

    public RoundRepository(IGameRoundsClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundInfo> GetAsync(
        Guid roundId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetGameRoundAsync(
                roundId: roundId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game round number could not be found.{roundId}");

        return response.Data is null
            ? throw new Exception($"This game round number could not be found.{roundId}")
            : new RoundInfo(
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
