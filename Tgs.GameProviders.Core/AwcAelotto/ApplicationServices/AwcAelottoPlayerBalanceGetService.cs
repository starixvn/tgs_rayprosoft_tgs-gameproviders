using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAelotto.ApplicationServices;

public class AwcAelottoPlayerBalanceGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceAwcClient client,
	IPlayerRepository playerRepository) : IGetService<AwcAelottoPlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(AwcAelottoPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_AELOTTO,
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

		var configuration = await gameProviderRepository.GetAwcAelottoConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currency = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await client.PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				cert: configuration.Cert,
				playerCode: query.PlayerCode,
				userName: query.PlayerCode,
				currency: currency,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				agentId: configuration.AgentId,
				cert: configuration.Cert,
				playerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.({player.PlayerCode})")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
