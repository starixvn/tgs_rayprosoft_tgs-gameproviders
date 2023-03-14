using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class GameProviderCacheRepository : IGameProviderRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IGameProviderRepository _repository;

    public GameProviderCacheRepository(
        IMemoryCache memoryCache,
        IGameProviderRepository repository)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
         => _repository.GetAsync(gameProviderId);

    public async ValueTask<AwcConfigurationData> GetAwcConfigurationData(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out AwcConfigurationData? configurationData))
        {
            configurationData = await _repository
                .GetAwcConfigurationData(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out BTiSportsConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetBTiSportsConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out DreamGamingTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetDreamGamingTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<EndorphinaConfiguration> GetEndorphinaConfiguration(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out EndorphinaConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetEndorphinaConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out FaChaiConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetFaChaiConfigurationAsync(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
        => _repository.GetGameEndpointsAsync(gameProviderConfigurationId);

    public async ValueTask<string> GetGameProviderCultureCodeAsync(
        Guid gameProviderId,
        string cultureCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:culturecode:{gameProviderId}:{cultureCode}";
        if (!_memoryCache.TryGetValue(key, out string? gameProviderCultureCode))
        {
            gameProviderCultureCode = await _repository
                .GetGameProviderCultureCodeAsync(
                    gameProviderId,
                    cultureCode,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: gameProviderCultureCode,
                        absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return gameProviderCultureCode ?? string.Empty;
    }

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:currencycode:{gameProviderId}:{currencyCode}";
        if (!_memoryCache.TryGetValue(key, out string? gameProviderCurrencyCode))
        {
            gameProviderCurrencyCode = await _repository
                .GetGameProviderCurrencyCodeAsync(
                    gameProviderId,
                    currencyCode,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
            {
                return _memoryCache
                     .Set(
                        key: key,
                        value: gameProviderCurrencyCode,
                        absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return gameProviderCurrencyCode ?? string.Empty;
    }

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:gameproviderid:{gameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out Guid? gameProviderId))
        {
            gameProviderId = await _repository
                .GetGameProviderIdAsync(
                    gameProviderCode: gameProviderCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (gameProviderId.HasValue)
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: gameProviderId.Value);
            }
        }
        return gameProviderId
            ?? throw new Exception($"Game Provider does not exist.({gameProviderCode})");
    }

    public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out HabaneroTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetHabaneroTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out JDBTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetJDBTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out JiliTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetJiliTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<SabaTransferConfiguration> GetSabaTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SabaTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetSabaTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SBOTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetSBOTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<SM01Configuration> GetSM01Configuration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SM01Configuration? configurationData))
        {
            configurationData = await _repository
                .GetSM01Configuration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out TFGamingTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetTFGamingTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out WGFmGamingConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetWGFmGamingConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out WMCasinoTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetWMCasinoTransferConfiguration(
                    gameProviderConfigurationId: gameProviderConfigurationId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _memoryCache
                .Set(
                    key: key,
                    value: configurationData,
                    absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
        }
        return configurationData
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
    }
}
