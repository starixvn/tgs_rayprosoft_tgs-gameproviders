using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingGameUrlGetService : IGetService<WGFmGamingGameUrlQuery, Uri>
{
	private readonly IBrandRepository _brandRepository;
	private readonly IGameInterfaceWGFmGamingClient _client;
	private readonly IGameProviderRepository _gameProviderRepository;
	private readonly IPlayerRepository _playerRepository;

	public WGFmGamingGameUrlGetService(
		IGameInterfaceWGFmGamingClient client,
		IGameProviderRepository gameProviderRepository,
		IPlayerRepository playerRepository,
		IBrandRepository brandRepository)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
		_playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
		_brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
	}

	public async ValueTask<Uri> GetAsync(WGFmGamingGameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProviderRepository
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WGFmGaming,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _playerRepository
			.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var gameProviderConfigurationId = await _brandRepository
			.GetBrandGameProviderConfigurationIdAsync(
			gameProviderId: gameProviderId,
			brandId: player.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var configurationData = await _gameProviderRepository
			.GetWGFmGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var cultureCode = await _gameProviderRepository
			.GetGameProviderCultureCodeAsync(
			gameProviderId: gameProviderId,
			cultureCode: query.Language,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false) ?? "2";
		var currencyCode = await _gameProviderRepository
			.GetGameProviderCurrencyCodeAsync(
			gameProviderId: gameProviderId,
			currencyCode: player.DefaultWalletCurrencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

		var token = await _client
			.GamePrepareLoginAsync(
			apiUrl: configurationData.ApiUrl,
			partnerCode: configurationData.PartnerCode,
			partnerKey: configurationData.PartnerKey,
			partnerIV: configurationData.IV,
			playerCode: player.PlayerCode,
			currencyCode: currencyCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await _client
			.GameLoginAsync(
			apiUrl: configurationData.ApiUrl,
			partnerCode: configurationData.PartnerCode,
			token: token,
			lang: cultureCode,
			version: configurationData.Version,
			query.PlatformType == PlatformType.Mobile,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
