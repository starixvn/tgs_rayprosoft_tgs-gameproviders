using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.VRGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.VRGaming.ApplicationServices;

public class VRGamingGameUrlGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameRepository game,
	IGameInterfaceVRGamingClient client) : IGetService<VRGamingGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(VRGamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProvider.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.VRGaming,
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
		var configuration = await gameProvider.GetVRGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameInfo = await game.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			version: configuration.Version,
			merchantId: configuration.MerchantId,
			deskey: configuration.Deskey,
			playerCode: playerInfo.PlayerCode,
			externalGameId: int.Parse(gameInfo.GameExternalId),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
