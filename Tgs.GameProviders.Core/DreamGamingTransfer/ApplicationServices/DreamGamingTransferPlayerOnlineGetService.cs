using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferPlayerOnlineGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceDreamGamingClient client,
	IPlayerRepository playerRepository) : IGetService<DreamGamingTransferPlayerOnlineQuery, bool>
{
	public async ValueTask<bool> GetAsync(DreamGamingTransferPlayerOnlineQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.DreamGamingTransfer,
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
		var configuration = await gameProviderRepository.GetDreamGamingTransferConfigurationAsync(
			gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerOnlines = await client.PlayerOnlineListAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			agentName: configuration.AgentName,
			cancellationToken: cancellationToken)
			.ToArrayAsync(cancellationToken)
			.ConfigureAwait(false);

		return playerOnlines.FirstOrDefault(p => p.PlayerCode == player.PlayerCode) is not null;
	}
}
