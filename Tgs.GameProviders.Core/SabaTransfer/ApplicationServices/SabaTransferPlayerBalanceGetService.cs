using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferPlayerBalanceGetService(
	IGameInterfaceSabaClient client,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository) : IGetService<SabaTransferPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(SabaTransferPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.SabaTransfer,
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
		var configuration = await gameProviderRepository.GetSabaTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			operatorId: configuration.OperatorId,
			vendorId: configuration.VendorId,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var gameProviderCurrencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				configuration.ApiUrl,
				operatorId: configuration.OperatorId,
				vendorId: configuration.VendorId,
				brandCode: player.BrandCode,
				playerCode: player.PlayerCode,
				oddsType: configuration.OddsType,
				currency: int.Parse(gameProviderCurrencyCode),
				cancellationToken)
			   .ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				operatorId: configuration.OperatorId,
				vendorId: configuration.VendorId,
				playerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance = reply.OutstandingBalance.ToDecimal().GetValueOrDefault(0),
			};
	}
}
