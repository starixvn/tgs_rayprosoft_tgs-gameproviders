using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.TFGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.TFGamingTransfer.ApplicationServices;

internal class TFGamingTransferCreatePlayerCommandService : ICommandService<TFGamingTransferCreatePlayerCommand>
{
	private readonly IBrandRepository _brand;
	private readonly IGameInterfaceTFGamingClient _client;
	private readonly IGameProviderRepository _gameProvider;
	private readonly IPlayerRepository _player;

	public TFGamingTransferCreatePlayerCommandService(
		IBrandRepository brand,
		IGameProviderRepository gameProvider,
		IPlayerRepository player,
		IGameInterfaceTFGamingClient client)
	{
		_brand = brand ?? throw new ArgumentNullException(nameof(brand));
		_gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask ExecuteAsync(TFGamingTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.TFGamingTransfer).ConfigureAwait(false);
		var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
		var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
		var configuration = await _gameProvider.GetTFGamingTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

		await _client.PlayerCreateAsync(
			apiUrl: configuration.ApiUrl,
			privateToken: configuration.PrivateToken,
			memberCode: command.PlayerCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
