using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TPhere.Client;

using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TPhere.ApplicationServices;

public class TPherePlayerFundCheckingGetService(
	IGameProviderRepository gameProvider,
	IPlayerRepository player,
	IBrandRepository brand,
	IGameInterfaceTPhereClient client) : IGetService<TPherePlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(TPherePlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
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

		return await client.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			apiKey: configuration.ApiKey,
			apiToken: configuration.ApiToken,
			orderId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
