using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketPlayerFundOutGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceWicketCricketClient client) : IGetService<WicketCricketPlayerFundOutQuery, PlayerFundResult>
{
	public async ValueTask<PlayerFundResult> GetAsync(WicketCricketPlayerFundOutQuery query, CancellationToken cancellationToken = default)
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

		_ = await client.FundOutAsync(
			apiUrl: configuration.ApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			playerCode: player.PlayerCode,
			orderId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			OrderId: query.OrderId,
			ExternalOrderId: string.Empty);
	}
}
