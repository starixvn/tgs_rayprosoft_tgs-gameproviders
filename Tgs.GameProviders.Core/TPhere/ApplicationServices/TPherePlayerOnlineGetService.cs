using System;

using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

public class TPherePlayerOnlineGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceTPhereClient client) : IGetService<TPherePlayerOnlineQuery, bool>
{
	public async ValueTask<bool> GetAsync(TPherePlayerOnlineQuery query, CancellationToken cancellationToken = default)
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

		return await client.TP2PlayerOnlineAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
