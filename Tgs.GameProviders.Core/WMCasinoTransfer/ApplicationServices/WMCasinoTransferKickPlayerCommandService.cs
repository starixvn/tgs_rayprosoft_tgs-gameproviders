using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices;

public class WMCasinoTransferKickPlayerCommandService : ICommandService<WMCasinoTransferKickPlayerCommand>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public WMCasinoTransferKickPlayerCommandService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceWMCasinoServiceGrpc grpc,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask ExecuteAsync(WMCasinoTransferKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.WMCasinoTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetWMCasinoTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var user = $"wmtf_{command.PlayerCode}_{player.BrandCode}";

        await _grpc.KickPlayerAsync(
            apiUrl: configuration.ApiUrl,
            vendorId: configuration.VendorId,
            signature: configuration.Signature,
            user: user,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
