using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.CQ9Gaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.CQ9Gaming.ApplicationServices;

public class CQ9GamingPlayerOnlineGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceCQ9GamingClient client) : IGetService<CQ9GamingPlayerOnlineQuery, bool>
{
	public async ValueTask<bool> GetAsync(CQ9GamingPlayerOnlineQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.CQ9Gaming,
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
		var configuration = await gameProvider.GetCQ9GamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.PlayerOnlineAsync(
			apiUrl: configuration.ApiUrl,
			apiToken: configuration.ApiToken,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
