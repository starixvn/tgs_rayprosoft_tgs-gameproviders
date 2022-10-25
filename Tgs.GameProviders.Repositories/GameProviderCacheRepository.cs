using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories;

internal class GameProviderCacheRepository : IGameProviderRepository
{
    private readonly IGameProviderRepository _gameProvider;
    private readonly IMemoryCache _memoryCache;

    public GameProviderCacheRepository(
        IMemoryCache memoryCache,
        IGameProviderRepository gameProvider)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
    }

    public ValueTask<GameProviderModel> GetAsync(Guid gameProviderId)
         => _gameProvider.GetAsync(gameProviderId);

    public async ValueTask<AwcConfigurationData> GetAwcConfigurationData(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out AwcConfigurationData configurationData))
        {
            configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
        => _gameProvider.GetGameEndpointsAsync(gameProviderConfigurationId);

    public async ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderId}:{cultureCode}".ToLower();
        if (!_memoryCache.TryGetValue(key, out string? gameProviderCultureCode))
        {
            gameProviderCultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
            {
                _ = _memoryCache.Set(key, gameProviderCultureCode, TimeSpan.FromHours(1));
            }
        }
        return gameProviderCultureCode;
    }

    public async ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderId}:{currencyCode}".ToLower();
        if (!_memoryCache.TryGetValue(key, out string? gameProviderCurrencyCode))
        {
            gameProviderCurrencyCode = await _gameProvider.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
            {
                _ = _memoryCache.Set(key, gameProviderCurrencyCode, TimeSpan.FromHours(1));
            }
        }
        return gameProviderCurrencyCode;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderCode}".ToLower();
        if (!_memoryCache.TryGetValue(key, out Guid gameProviderId))
        {
            gameProviderId = await _gameProvider.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);
            _ = _memoryCache.Set(key, gameProviderId);
        }
        return gameProviderId;
    }

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out JiliTransferConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetJiliTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out WMCasinoTransferConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetWMCasinoTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out WGFmGamingConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetWGFmGamingConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out JDBTransferConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetJDBTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out SBOTransferConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetSBOTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out BTiSportsConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetBTiSportsConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out HabaneroTransferConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetHabaneroTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }
}
