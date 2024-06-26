using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.YeeBet.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.YeeBet.ApplicationServices;

public class YeeBetPlayerFundCheckingGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceYeeBetClient client) : IGetService<YeeBetPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(YeeBetPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.YeeBet,
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
		var configuration = await gameProviderRepository.GetYeeBetConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			appId: configuration.AppId,
			secretKey: configuration.SecretKey,
			playerCode: query.PlayerCode,
			orderId: long.Parse(query.OrderId),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
