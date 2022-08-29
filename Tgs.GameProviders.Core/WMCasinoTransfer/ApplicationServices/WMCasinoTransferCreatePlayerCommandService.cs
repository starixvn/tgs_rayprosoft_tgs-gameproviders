using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

internal class WMCasinoTransferCreatePlayerCommandService : ICommandService<WMCasinoTransferCreatePlayerCommand>
{
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly IPlayerRepository _player;

    public WMCasinoTransferCreatePlayerCommandService(
        IPlayerRepository player,
        IGameInterfaceWMCasinoServiceGrpc grpc)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
    }

    public async ValueTask ExecuteAsync(WMCasinoTransferCreatePlayerCommand command, CancellationToken cancellationToken = default)
    {
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);

        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";

        await _grpc.CreatePlayerAsync(
            apiUrl: command.Configuration.ApiUrl,
            vendorId: command.ConfigurationData.VendorId,
            signature: command.ConfigurationData.Signature,
            user: user,
            password: command.Password,
            userName: command.PlayerCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
