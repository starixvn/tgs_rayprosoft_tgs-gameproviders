using System;

using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

public class TPherePlayerBalanceGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceTPhereClient client) : IGetService<TPherePlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(TPherePlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.TPhere,
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
		var configuration = await gameProvider.GetTPhereConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await client.TP2PlayerBalanceAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			await client.TP2PlayerCreateAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiToken: configuration.ApiToken,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await client.TP2PlayerBalanceAsync(
				apiUrl: configuration.ApiUrl,
				apiKey: configuration.ApiKey,
				apiToken: configuration.ApiToken,
				playerCode: playerInfo.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return reply.IsPlayerExist
			? new PlayerBalanceResult
			{
				FreeBalance = reply.Balance
			}
			: throw new Exception($"player does not exist.({playerInfo.PlayerCode})");
	}
}
