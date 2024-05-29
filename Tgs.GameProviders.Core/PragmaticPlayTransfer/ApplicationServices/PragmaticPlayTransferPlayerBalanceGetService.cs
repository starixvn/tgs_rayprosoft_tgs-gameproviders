using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.PragmaticPlay.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;

internal class PragmaticPlayTransferPlayerBalanceGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfacePragmaticPlayClient client) : IGetService<PragmaticPlayTransferPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(
		PragmaticPlayTransferPlayerBalanceQuery query,
		CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.PragmaticPlayTransfer,
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
		var configuration = await gameProvider.GetPragmaticPlayTransferConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			configuration.ApiUrl,
			configuration.SecureLogin,
			configuration.SecretKey,
			query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currencyCode = await gameProvider.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: playerInfo.DefaultWalletCurrencyCode,
				cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				secureLogin: configuration.SecureLogin,
				secretKey: configuration.SecretKey,
				playerCode: query.PlayerCode,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				configuration.ApiUrl,
				configuration.SecureLogin,
				configuration.SecretKey,
				query.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{playerInfo.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
