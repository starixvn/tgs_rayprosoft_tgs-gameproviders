using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AsiaGaming;

internal class AsiaGamingService : IAsiaGamingService
{
    private readonly IBrandService _brandService;
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<AsiaGamingService> _logger;
    private readonly ISettingService _settingService;

    public AsiaGamingService(
        ILogger<AsiaGamingService> logger,
        ISettingService settingService,
        IGameProviderRepository gameProviderRepository,
        IGameService gameService,
        IBrandService brandService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
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

        var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
            .GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

        return platformType == PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
            .ConfigureAwait(false);

        return isCurrencyCode ? gameProviderCurrencyCode : null;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync()
        => await _gameProviderRepository.GetGameProviderIdAsync(Domains.GameProviderType.AsiaGaming)
            .ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<AsiaGamingConfiguration>(Domains.GameProviderConfigurations.AsiaGaming)
            .ConfigureAwait(false);
}
