using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;

public class WGViaGamingPlayerBalanceGetService : IGetService<WGViaGamingPlayerBalanceQuery, PlayerBalanceResult>
{
	private readonly IBrandRepository _brandRepository;
	private readonly IGameInterfaceWGViaGamingClient _client;
	private readonly IGameProviderRepository _gameProviderRepository;
	private readonly IPlayerRepository _playerRepository;

	public WGViaGamingPlayerBalanceGetService(
		IGameProviderRepository gameProviderRepository,
		IPlayerRepository playerRepository,
		IBrandRepository brandRepository,
		IGameInterfaceWGViaGamingClient client
		)
	{
		_gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
		_playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
		_brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<PlayerBalanceResult> GetAsync(WGViaGamingPlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProviderRepository
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WGViaGaming,
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
			.GetWGViaGamingConfigurationAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var reply = await _client
			 .PlayerBalanceAsync(
			 apiUrl: configurationData.ApiUrl,
			 partnerCode: configurationData.PartnerCode,
			 secretKey: configurationData.PartnerKey,
			 secretIV: configurationData.IV,
			 playerCode: player.PlayerCode,
			 cancellationToken: cancellationToken)
			 .ConfigureAwait(false);

		if (!reply.IsPlayerExist)
		{
			var currencyCode = await _gameProviderRepository
				.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: player.DefaultWalletCurrencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new Exception($"This game product currency does not exist.({player.DefaultWalletCurrencyCode})");

			await _client
				.PlayerCreateAsync(
				apiUrl: configurationData.ApiUrl,
				partnerCode: configurationData.PartnerCode,
				secretKey: configurationData.PartnerKey,
				secretIV: configurationData.IV,
				playerCode: player.PlayerCode,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			reply = await _client
				.PlayerBalanceAsync(
				apiUrl: configurationData.ApiUrl,
				partnerCode: configurationData.PartnerCode,
				secretKey: configurationData.PartnerKey,
				secretIV: configurationData.IV,
				playerCode: player.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return !reply.IsPlayerExist
			? throw new Exception($"Player does not exist.{player.PlayerCode}")
			: new PlayerBalanceResult
			{
				FreeBalance = reply.Balance.ToDecimal().GetValueOrDefault(0)
			};
	}
}
