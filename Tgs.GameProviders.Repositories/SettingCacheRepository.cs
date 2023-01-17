using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class SettingCacheRepository : ISettingRepository
{
    private readonly TimeSpan _absoluteExpiration;
    private readonly IMemoryCache _memoryCache;
    private readonly ISettingRepository _setting;

    public SettingCacheRepository(
        IMemoryCache memoryCache,
        ISettingRepository setting)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        _absoluteExpiration = TimeSpan.FromMinutes(5);
    }

    public async ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_AELOTTO);
        if (!_memoryCache.TryGetValue(key, out AwcAelottoConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcAelottoConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_AWS);
        if (!_memoryCache.TryGetValue(key, out AwcAwsConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcAwsConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_COCKFIGHT);
        if (!_memoryCache.TryGetValue(key, out AwcCockfightConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcCockfightConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_FACAI);
        if (!_memoryCache.TryGetValue(key, out AwcFacaiConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcFacaiConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_FS);
        if (!_memoryCache.TryGetValue(key, out AwcFsConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcFsConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_KM);
        if (!_memoryCache.TryGetValue(key, out AwcKmConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcKmConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_LUDO);
        if (!_memoryCache.TryGetValue(key, out AwcLudoConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcLudoConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_PGS);
        if (!_memoryCache.TryGetValue(key, out AwcPgsConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcPgsConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_RCB);
        if (!_memoryCache.TryGetValue(key, out AwcRcbConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcRcbConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_SG);
        if (!_memoryCache.TryGetValue(key, out AwcSgConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcSgConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_SPADE);
        if (!_memoryCache.TryGetValue(key, out AwcSpadeConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcSpadeConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_VENUS);
        if (!_memoryCache.TryGetValue(key, out AwcVenusConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcVenusConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.AWC_YL);
        if (!_memoryCache.TryGetValue(key, out AwcYlConfiguration? configuration))
        {
            configuration = await _setting
                .GetAwcYlConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default) where TConfiguration : PluggableConfigurationBase
        => _setting.GetConfigurationAsync<TConfiguration>(key);

    public async ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.EvolutionTransfer);
        if (!_memoryCache.TryGetValue(key, out EvolutionTransferConfiguration? configuration))
        {
            configuration = await _setting
                .GetEvolutionTransferConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.IDNLive);
        if (!_memoryCache.TryGetValue(key, out IDNLiveConfiguration? configuration))
        {
            configuration = await _setting
                .GetIDNLiveConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    public async ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync(
        CancellationToken cancellationToken = default)
    {
        var key = GetKey(GameProviderConfigurations.SkywindTransfer);
        if (!_memoryCache.TryGetValue(key, out SkywindTransferConfiguration? configuration))
        {
            configuration = await _setting
                .GetSkywindTransferConfigurationAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            return _memoryCache
                .Set(
                    key,
                    configuration,
                    _absoluteExpiration);
        }
        return configuration!;
    }

    private static string GetKey(string gameProviderCode)
            => $"gameg:gameproviders:settingcache:{gameProviderCode}";
}
