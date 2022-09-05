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

    public async ValueTask<JiliTransferConfigurationData> GetJiliTransferConfigurationData(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out JiliTransferConfigurationData configurationData))
        {
            configurationData = await _gameProvider.GetJiliTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<WMCasinoTransferConfigurationData> GetWMCasinoTransferConfigurationData(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out WMCasinoTransferConfigurationData configurationData))
        {
            configurationData = await _gameProvider.GetWMCasinoTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationData(Guid gameProviderConfigurationId)
    {
        var key = $"GameProviders:GameProviderCache:{gameProviderConfigurationId}".ToLower();
        if (!_memoryCache.TryGetValue(key, out WGFmGamingConfiguration configurationData))
        {
            configurationData = await _gameProvider.GetWGFmGamingConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            _ = _memoryCache.Set(key, configurationData, TimeSpan.FromMinutes(5));
        }
        return configurationData;
    }
}
