using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.EndorphinaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Endorphina.ApplicationServices;

internal class EndorphinaPlayerFundCheckingGetService(
	IBrandRepository brandRepository,
	IGameProviderRepository gameProviderRepository,
	IGameInterfaceEndorphinaClient client,
	IPlayerRepository playerRepository) : IGetService<EndorphinaPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(EndorphinaPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.Endorphina,
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
		var configuration = await gameProviderRepository.GetEndorphinaConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return (await client.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			merchantId: configuration.MerchantId,
			secretKey: configuration.SecretKey,
			transactionId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.IsOrderExist;
	}
}
