using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGViaGaming.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGViaGaming.ApplicationServices;

public class WGViaGamingKickPlayerCommandService : ICommandService<WGViaGamingKickPlayerCommand>
{
	private readonly IBrandRepository _brandRepository;
	private readonly IGameInterfaceWGViaGamingClient _client;
	private readonly IGameProviderRepository _gameProviderRepository;
	private readonly IPlayerRepository _playerRepository;

	public WGViaGamingKickPlayerCommandService(
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

	public async ValueTask ExecuteAsync(WGViaGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProviderRepository
			.GetGameProviderIdAsync(
			gameProviderCode: GameProviderType.WGViaGaming,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		var player = await _playerRepository
			.GetAsync(
			playerCode: command.PlayerCode,
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

		await _client
			.PlayerLogoutAsync(
			apiUrl: configurationData.ApiUrl,
			partnerCode: configurationData.PartnerCode,
			secretKey: configurationData.PartnerKey,
			secretIV: configurationData.IV,
			playerCode: player.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
