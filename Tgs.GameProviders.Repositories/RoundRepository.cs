using Tgs.GameProviders.Core;
using Tgs.GameRounds.Client;

namespace Tgs.GameProviders.Repositories;

internal class RoundRepository(IGameRoundsClient client) : IRoundRepository
{
	public async ValueTask<RoundInfo> GetAsync(Guid roundId, CancellationToken cancellationToken = default)
	{
		var response = await client.GetGameRoundAsync(
			roundId: roundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(roundId.ToString(), "This game bet does not exist.");

		return new RoundInfo(
			RoundId: response.RoundId,
			ExternalRoundId: response.ExternalRoundId,
			GameProviderId: response.GameProviderId,
			PlayerCode: response.PlayerCode,
			GameId: response.GameId,
			CurrencyCode: response.CurrencyCode,
			Status: response.Status,
			GameProviderConfigurationId: response.GameProviderConfigurationId,
			WalletId: response.WalletId,
			CreatedOn: response.CreatedOn,
			ClosedOn: response.ClosedOn,
			UpdatedOn: response.UpdatedOn,
			BrandId: response.BrandId,
			ExternalCreatedOn: response.ExternalCreatedOn,
			ExternalClosedOn: response.ExternalClosedOn,
			BetAmount: response.BetAmount,
			ValidBetAmount: response.ValidBetAmount,
			SettleAmount: response.SettleAmount,
			Commission: response.Commission,
			Turnover: response.Turnover,
			CompanyWinLoss: response.CompanyWinLoss,
			TieBetAdjustment: response.TieBetAdjustment,
			ClientStatus: response.ClientStatus,
			ReopenTurnoverAdjustment: response.ReopenTurnoverAdjustment,
			ReopenedOn: response.ReopenedOn,
			EffectiveBettingPercentage: response.EffectiveBettingPercentage,
			AdditionalData: response.AdditionalData);
	}
}
