using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.DreamGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.DreamGamingTransfer.ApplicationServices;

public class DreamGamingTransferKickPlayerCommandService : ICommandService<DreamGamingTransferKickPlayerCommand>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceDreamGamingClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public DreamGamingTransferKickPlayerCommandService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceDreamGamingClient client,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask ExecuteAsync(DreamGamingTransferKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.DreamGamingTransfer,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var player = await _player
            .GetAsync(
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand
            .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: player.BrandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var configuration = await _gameProvider
            .GetDreamGamingTransferConfigurationAsync(
            gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerOnlines = await _client.PlayerOnlineAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            agentName: configuration.AgentName,
            cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        var playerOnline = playerOnlines.FirstOrDefault(p => p.PlayerCode == command.PlayerCode);

        if (playerOnline == null)
            return;

        await _client.PlayerKickAsync(
            apiUrl: configuration.ApiUrl,
            apiKey: configuration.ApiKey,
            agentName: configuration.AgentName,
            memberId: playerOnline.MemberId,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
