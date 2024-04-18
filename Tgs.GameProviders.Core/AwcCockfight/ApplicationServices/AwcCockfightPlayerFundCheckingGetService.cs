using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.Awc.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;

public class AwcCockfightPlayerFundCheckingGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceAwcClient client,
	IPlayerRepository playerRepository) : IGetService<AwcCockfightPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(AwcCockfightPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.AWC_COCKFIGHT,
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

		var configuration = await gameProviderRepository.GetAwcCockfightConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client
			.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			agentId: configuration.AgentId,
			cert: configuration.Cert,
			orderId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
