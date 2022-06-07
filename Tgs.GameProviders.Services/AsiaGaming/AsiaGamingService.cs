using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AsiaGaming;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.AsiaGaming;

internal class AsiaGamingService : IAsiaGamingService
{
    private readonly IBrandService _brandService;
    private readonly IGameProviderRepository _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ILogger<AsiaGamingService> _logger;
    private readonly ISettingRepository _settingService;

    public AsiaGamingService(
        ILogger<AsiaGamingService> logger,
        ISettingRepository settingService,
        IGameProviderRepository gameProviderService,
        IGameService gameService,
        IBrandService brandService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    }

    public async ValueTask<Guid> GetBrandIdAsync(string brandCode)
        => await _brandService.GetBrandIdAsync(brandCode).ConfigureAwait(false);

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameService.GetAsync(gameId).ConfigureAwait(false);

    public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType)
    {
        var gameProviderConfigurationId = await _brandService
            .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId).ConfigureAwait(false);

        var (desktopEndpoint, mobileEndpoint) = await _gameProviderService
            .GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

        return platformType == PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync()
        => await _gameProviderService.GetGameProviderIdAsync(Domains.GameProviderType.AsiaGaming)
            .ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<AsiaGamingConfiguration>(Domains.GameProviderConfigurations.AsiaGaming)
            .ConfigureAwait(false);
}
