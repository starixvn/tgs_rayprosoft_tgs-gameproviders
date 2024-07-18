using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketGameUrlGetService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandRepository brandRepository,
	IGameInterfaceWicketCricketClient client) : IGetService<WicketCricketGameUrlQuery, string>
{
	public async ValueTask<string> GetAsync(WicketCricketGameUrlQuery query, CancellationToken cancellationToken = default)
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
		var currencyCode = await gameProviderRepository.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken)
			.ConfigureAwait(false);

		return await client.GameUrlAsync(
			apiUrl: configuration.ApiUrl,
			exchangeApiUrl: configuration.ExchangeApiUrl,
			website: configuration.Website,
			cert: configuration.Cert,
			agent: configuration.Agent,
			playerCode: player.PlayerCode,
			currency: int.Parse(currencyCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
