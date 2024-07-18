using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketPlayerBalanceGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceWicketCricketClient client) : IGetService<WicketCricketPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(WicketCricketPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WicketCricket,
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
		var configuration = await gameProviderRepository.GetWicketCricketConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				website: configuration.Website,
				cert: configuration.Cert,
				agent: configuration.Agent,
				playerCode: player.PlayerCode,
				currency: int.Parse(currencyCode),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
			   apiUrl: configuration.ApiUrl,
			   website: configuration.Website,
			   cert: configuration.Cert,
			   playerCode: player.PlayerCode,
			   cancellationToken: cancellationToken)
			   .ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.FreeBalance.ToDecimal().GetValueOrDefault(0),
				OutstandingBalance = reply.OutstandingBalance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
