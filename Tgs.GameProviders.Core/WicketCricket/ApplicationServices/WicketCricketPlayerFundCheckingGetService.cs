using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketPlayerFundCheckingGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceWicketCricketClient client) : IGetService<WicketCricketPlayerFundCheckingQuery, bool>
{
	public async ValueTask<bool> GetAsync(WicketCricketPlayerFundCheckingQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WicketCricket,
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
		var configuration = await gameProviderRepository.GetWicketCricketConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await client.FundStatusAsync(
			apiUrl: configuration.ApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			orderId: query.OrderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
