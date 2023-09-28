using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WicketCricket.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WicketCricket.ApplicationServices;

public class WicketCricketKickPlayerCommandService : ICommandService<WicketCricketKickPlayerCommand>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceWicketCricketClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;

    public WicketCricketKickPlayerCommandService(
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IBrandRepository brand,
        IGameInterfaceWicketCricketClient client)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask ExecuteAsync(WicketCricketKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.WicketCricket,
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
            .GetWicketCricketConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await _client
            .PlayerLogoutAsync(
            exchangeApiUrl: configuration.ExchangeApiUrl,
            website: configuration.Website,
            cert: configuration.Cert,
            playerCode: player.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
