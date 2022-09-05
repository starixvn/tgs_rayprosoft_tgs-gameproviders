using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;

public class WGFmGamingKickPlayerCommandService : ICommandService<WGFmGamingKickPlayerCommand>
{
    private readonly IGameInterfaceWGFmGamingClient _client;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IBrandRepository _brandRepository;

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
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
        var player = await _playerRepository.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brandRepository.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProviderRepository.GetWGFmGamingConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        await _client.PlayerKickAsync(
            configurationData.ApiUrl,
            configurationData.PartnerCode,
            configurationData.PartnerKey,
            configurationData.IV,
            command.PlayerCode,
            cancellationToken).ConfigureAwait(false);
    }
}
