using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.GlobalTechnology.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.GTExpanse.ApplicationServices;

public class GTExpansePlayerFundInGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceGlobalTechnologyClient client) : IGetService<GTExpansePlayerFundInQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(GTExpansePlayerFundInQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.GT_Expanse,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await player.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brand.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProvider.GetGTExpanseConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var currencyCode = await gameProvider.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundInAsync(
			apiUrl: configuration.ApiUrl,
			brandId: configuration.BrandId,
			secretKey: configuration.SecretKey,
			playerCode: playerInfo.PlayerCode,
			currency: currencyCode,
			orderId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: reply.OrderId,
			ExternalOrderId: reply.ExternalOrderId);
	}
}
