using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Kaiyuan.ApplicationServices;

public class KaiyuanGameUrlGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameRepository gameRepository,
	IGameInterfaceKaiyuanClient client) : IGetService<KaiyuanGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(KaiyuanGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Kaiyuan,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await brandRepository.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: playerInfo.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configuration = await gameProviderRepository.GetKaiyuanConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var game = await gameRepository.GetAsync(
			gameId: query.GameId.Value,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			brandCode: playerInfo.BrandCode,
			playerCode: playerInfo.PlayerCode,
			ipAddress: query.IPAddress,
			externalGameId: game.GameExternalId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
