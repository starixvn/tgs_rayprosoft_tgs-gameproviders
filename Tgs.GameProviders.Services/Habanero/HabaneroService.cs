using System;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Habanero;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Habanero;

internal class HabaneroService : IHabaneroService
{
    private readonly IBrandRepository _brandService;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ISettingRepository _settingRepository;

    public HabaneroService(
        IBrandRepository brandService,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IPlayerRepository playerRepository,
        ISettingRepository settingRepository)
    {
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

    public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, Core.PlatformType platformType)
    {
        var configurationId = await _brandService
            .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
            .ConfigureAwait(false);

        var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
            .GetGameEndpointsAsync(configurationId)
            .ConfigureAwait(false);

        return platformType == Core.PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
    }

    public ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderRepository.GetAsync(gameProviderId);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<PlayerInfo> GetPlayerAsync(string playerCode)
        => await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);

    public async ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<HabaneroConfiguration>(GameProviderConfigurations.Habanero)
            .ConfigureAwait(false);
}
