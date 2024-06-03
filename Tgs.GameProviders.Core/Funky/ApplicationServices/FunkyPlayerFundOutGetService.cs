using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Funky.Client;

using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Funky.ApplicationServices;

public class FunkyPlayerFundOutGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceFunkyClient client) : IGetService<FunkyPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(FunkyPlayerFundOutQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Funky,
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
		var configuration = await gameProvider.GetFunkyConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var currency = await gameProvider.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: playerInfo.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.FundOutAsync(
			apiUrl: configuration.ApiUrl,
			userAgent: configuration.UserAgent,
			authentication: configuration.Authentication,
			playerCode: playerInfo.PlayerCode,
			currency: currency,
			orderId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: reply.TxSerialId.ToString());
	}
}
