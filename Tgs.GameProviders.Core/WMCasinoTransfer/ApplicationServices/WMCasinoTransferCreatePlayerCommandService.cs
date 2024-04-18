using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

internal class WMCasinoTransferCreatePlayerCommandService : ICommandService<WMCasinoTransferCreatePlayerCommand>
{
	private readonly IGameInterfaceWMCasinoServiceClient _client;
	private readonly IPlayerRepository _player;

	public WMCasinoTransferCreatePlayerCommandService(
		IPlayerRepository player,
		IGameInterfaceWMCasinoServiceClient client)
	{
		_player = player ?? throw new ArgumentNullException(nameof(player));
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask ExecuteAsync(WMCasinoTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
	{
		var player = await _player
			.GetAsync(
				playerCode: command.PlayerCode,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";

		await _client.CreatePlayerAsync(
			apiUrl: command.Configuration.ApiUrl,
			vendorId: command.Configuration.VendorId,
			signature: command.Configuration.Signature,
			user: user,
			password: command.Password,
			userName: command.PlayerCode,
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
