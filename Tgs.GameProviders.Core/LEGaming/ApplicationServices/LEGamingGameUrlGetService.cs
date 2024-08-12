using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.LEGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.LEGaming.ApplicationServices;

public class LEGamingGameUrlGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameRepository gameRepository,
	IGameInterfaceLEGamingClient client) : IGetService<LEGamingGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(LEGamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.LEGaming,
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
		var configuration = await gameProviderRepository.GetLEGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var game = await gameRepository.GetAsync(
			gameId: query.GameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			deskey: configuration.Deskey,
			md5key: configuration.Md5key,
			agentId: configuration.AgentId,
			agentCurrency: configuration.AgentCurrency,
			brandCode: player.BrandCode,
			playerCode: player.PlayerCode,
			ipAddress: query.IPAddress,
			externalGameId: game.GameExternalId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
