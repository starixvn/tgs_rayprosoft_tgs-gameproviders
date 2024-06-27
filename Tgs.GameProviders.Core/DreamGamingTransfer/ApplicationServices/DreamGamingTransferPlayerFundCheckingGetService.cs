using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferPlayerFundCheckingGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceDreamGamingClient client,
	IPlayerRepository playerRepository) : IGetService<DreamGamingTransferPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(DreamGamingTransferPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
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

		return (await client.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			agentName: configuration.AgentName,
			orderId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.IsOrderExist;
	}
}
