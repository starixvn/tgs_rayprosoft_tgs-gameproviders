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
         => _repository
        .GetAsync(
             gameProviderId: gameProviderId,
             cancellationToken: cancellationToken);

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

    public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out BTiSportsConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetBTiSportsConfigurationAsync(
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

    public async ValueTask<Cards365TransferConfiguration> GetCards365TransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out Cards365TransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetCards365TransferConfigurationAsync(
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

    public async ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out DreamGamingTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetDreamGamingTransferConfigurationAsync(
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

    public async ValueTask<EndorphinaConfiguration> GetEndorphinaConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out EndorphinaConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetEndorphinaConfigurationAsync(
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
        => _repository
        .GetGameEndpointsAsync(
            gameProviderConfigurationId: gameProviderConfigurationId,
            cancellationToken: cancellationToken);

    public async ValueTask<string?> GetGameProviderCultureCodeAsync(
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
                .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: gameProviderCultureCode,
                        absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return gameProviderCultureCode;
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
                .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
            {
                return _memoryCache
                     .Set(
                        key: key,
                        value: gameProviderCurrencyCode,
                        absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
            }
        }
        return gameProviderCurrencyCode
            ?? throw new Exception($"This game product currency does not exist.({currencyCode})");
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

    public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out HabaneroTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetHabaneroTransferConfigurationAsync(
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

    public async ValueTask<InplayMatrixESportsConfiguration> GetInplayMatrixESportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out InplayMatrixESportsConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetInplayMatrixESportsConfigurationAsync(
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

    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out JDBTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetJDBTransferConfigurationAsync(
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

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out JiliTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetJiliTransferConfigurationAsync(
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

    public async ValueTask<JokerConfiguration> GetJokerConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out JokerConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetJokerConfigurationAsync(
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

    public async ValueTask<OBetConfiguration> GetOBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out OBetConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetOBetConfigurationAsync(
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

    public async ValueTask<PinnacleConfiguration> GetPinnacleConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out PinnacleConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetPinnacleConfigurationAsync(
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

    public async ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out PragmaticPlayTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetPragmaticPlayTransferConfigurationAsync(
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

    public async ValueTask<SabaTransferConfiguration> GetSabaTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SabaTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetSabaTransferConfigurationAsync(
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

    public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SBOTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetSBOTransferConfigurationAsync(
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

    public async ValueTask<SM01Configuration> GetSM01ConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out SM01Configuration? configurationData))
        {
            configurationData = await _repository
                .GetSM01ConfigurationAsync(
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

    public async ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out TFGamingTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetTFGamingTransferConfigurationAsync(
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

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out WGFmGamingConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetWGFmGamingConfigurationAsync(
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

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out WMCasinoTransferConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetWMCasinoTransferConfigurationAsync(
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

    public async ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
        if (!_memoryCache.TryGetValue(key, out YeeBetConfiguration? configurationData))
        {
            configurationData = await _repository
                .GetYeeBetConfigurationAsync(
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
