using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferKickPlayerCommandService : ICommandService<JiliTransferKickPlayerCommand>
{
    private readonly IBrandService _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceJiliServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public JiliTransferKickPlayerCommandService(
        IBrandService brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceJiliServiceGrpc grpc,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask ExecuteAsync(JiliTransferKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.JiliTransfer).ConfigureAwait(false);
        var configuration = await _setting.GetJiliTransferConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetJiliTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        await _grpc.KickPlayerAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configurationData.AgentId,
            agentKey: configuration.AgentKey,
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
