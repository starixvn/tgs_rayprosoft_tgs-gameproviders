using System;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.AsiaGaming;

internal class AsiaGamingService : IAsiaGamingService
{
    private readonly IBrandRepository _brandService;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ISettingRepository _settingRepository;

    public AsiaGamingService(
        ISettingRepository settingRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        IBrandRepository brandService)
    {
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    }

    public async ValueTask<Guid> GetBrandIdAsync(string brandCode)
        => await _brandService.GetBrandIdAsync(brandCode).ConfigureAwait(false);

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

    public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType)
    {
        var gameProviderConfigurationId = await _brandService
            .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId).ConfigureAwait(false);

        var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
            .GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

        return platformType == PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync()
        => await _gameProviderRepository.GetGameProviderIdAsync(Domains.GameProviderType.AsiaGaming)
            .ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<AsiaGamingConfiguration>(Domains.GameProviderConfigurations.AsiaGaming)
            .ConfigureAwait(false);
}
