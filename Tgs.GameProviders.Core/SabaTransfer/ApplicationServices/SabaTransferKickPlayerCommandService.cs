using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.SabaTransfer.ApplicationServices;

public class SabaTransferKickPlayerCommandService : ICommandService<SabaTransferKickPlayerCommand>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceSabaClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public SabaTransferKickPlayerCommandService(
        IGameInterfaceSabaClient client,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
    }

    public async ValueTask ExecuteAsync(SabaTransferKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.SabaTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetSabaTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        await _client
            .PlayerKickAsync(
                apiUrl: configuration.ApiUrl,
                operatorId: configuration.OperatorId,
                vendorId: configuration.VendorId,
                vendorMemberId: player.PlayerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
