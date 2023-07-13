using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingKickPlayerCommandService : ICommandService<WGFmGamingKickPlayerCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;

    public WGFmGamingKickPlayerCommandService(
        IGameInterfaceWGFmGamingClient client,
        IGameProviderRepository gameProviderRepository,
        IPlayerRepository playerRepository,
        IBrandRepository brandRepository)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
    }

    public async ValueTask ExecuteAsync(WGFmGamingKickPlayerCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProviderRepository
            .GetGameProviderIdAsync(
            gameProviderCode: GameProviderType.WGFmGaming,
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
            .GetWGFmGamingConfigurationAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await _client
            .PlayerKickAsync(
            apiUrl: configurationData.ApiUrl,
            partnerCode: configurationData.PartnerCode,
            partnerKey: configurationData.PartnerKey,
            partnerIV: configurationData.IV,
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
