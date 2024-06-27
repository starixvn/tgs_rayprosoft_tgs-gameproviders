using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaPlayerBalanceGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceEndorphinaClient client,
	IPlayerRepository playerRepository) : IGetService<EndorphinaPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(EndorphinaPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Endorphina,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetEndorphinaConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var currency = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			merchantId: configuration.MerchantId,
			secretKey: configuration.SecretKey,
			username: player.PlayerCode,
			currency: currency,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				merchantId: configuration.MerchantId,
				secretKey: configuration.SecretKey,
				username: player.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				merchantId: configuration.MerchantId,
				secretKey: configuration.SecretKey,
				username: player.PlayerCode,
				currency: currency,
				cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
