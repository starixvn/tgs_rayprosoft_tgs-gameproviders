using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;

public class WGViaGamingPlayerFundInGetService : IGetService<WGViaGamingPlayerFundInQuery, PlayerFundResult>
{
	private readonly IBrandRepository _brandRepository;
	private readonly IGameInterfaceWGViaGamingClient _client;
	private readonly IGameProviderRepository _gameProviderRepository;
	private readonly IPlayerRepository _playerRepository;

	public WGViaGamingPlayerFundInGetService(
		IGameProviderRepository gameProviderRepository,
		IPlayerRepository playerRepository,
		IBrandRepository brandRepository,
		IGameInterfaceWGViaGamingClient client)
	{
		_gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
		_playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
		_brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<PlayerFundResult> GetAsync(WGViaGamingPlayerFundInQuery query, CancellationToken cancellationToken = default)
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

		await _client.FundInAsync(
			apiUrl: configurationData.ApiUrl,
			partnerCode: configurationData.PartnerCode,
			secretKey: configurationData.PartnerKey,
			secretIV: configurationData.IV,
			playerCode: player.PlayerCode,
			orderId: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new PlayerFundResult(
			GameProviderTransactionId: query.OrderId,
			ExternalGameProviderTransactionId: string.Empty);
	}
}
