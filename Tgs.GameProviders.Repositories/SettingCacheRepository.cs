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
        _absoluteExpiration = TimeSpan.FromHours(1);
    }

    public async ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_AELOTTO);
        if (!_memoryCache.TryGetValue(key, out AwcAelottoConfiguration configuration))
        {
            configuration = await _setting.GetAwcAelottoConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_AWS);
        if (!_memoryCache.TryGetValue(key, out AwcAwsConfiguration configuration))
        {
            configuration = await _setting.GetAwcAwsConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_COCKFIGHT);
        if (!_memoryCache.TryGetValue(key, out AwcCockfightConfiguration configuration))
        {
            configuration = await _setting.GetAwcCockfightConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_FACAI);
        if (!_memoryCache.TryGetValue(key, out AwcFacaiConfiguration configuration))
        {
            configuration = await _setting.GetAwcFacaiConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_FS);
        if (!_memoryCache.TryGetValue(key, out AwcFsConfiguration configuration))
        {
            configuration = await _setting.GetAwcFsConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_KM);
        if (!_memoryCache.TryGetValue(key, out AwcKmConfiguration configuration))
        {
            configuration = await _setting.GetAwcKmConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_LUDO);
        if (!_memoryCache.TryGetValue(key, out AwcLudoConfiguration configuration))
        {
            configuration = await _setting.GetAwcLudoConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_PGS);
        if (!_memoryCache.TryGetValue(key, out AwcPgsConfiguration configuration))
        {
            configuration = await _setting.GetAwcPgsConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_RCB);
        if (!_memoryCache.TryGetValue(key, out AwcRcbConfiguration configuration))
        {
            configuration = await _setting.GetAwcRcbConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_SG);
        if (!_memoryCache.TryGetValue(key, out AwcSgConfiguration configuration))
        {
            configuration = await _setting.GetAwcSgConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_SPADE);
        if (!_memoryCache.TryGetValue(key, out AwcSpadeConfiguration configuration))
        {
            configuration = await _setting.GetAwcSpadeConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_VENUS);
        if (!_memoryCache.TryGetValue(key, out AwcVenusConfiguration configuration))
        {
            configuration = await _setting.GetAwcVenusConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.AWC_YL);
        if (!_memoryCache.TryGetValue(key, out AwcYlConfiguration configuration))
        {
            configuration = await _setting.GetAwcYlConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
        => _setting.GetConfigurationAsync<TConfiguration>(key);

    public async ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.EvolutionTransfer);
        if (!_memoryCache.TryGetValue(key, out EvolutionTransferConfiguration configuration))
        {
            configuration = await _setting.GetEvolutionTransferConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<IDNLiveConfiguration> GetIDNLiveConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.IDNLive);
        if (!_memoryCache.TryGetValue(key, out IDNLiveConfiguration configuration))
        {
            configuration = await _setting.GetIDNLiveConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.JiliTransfer);
        if (!_memoryCache.TryGetValue(key, out JiliTransferConfiguration configuration))
        {
            configuration = await _setting.GetJiliTransferConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<SWTransferConfiguration> GetSkywindTransferConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.SkywindTransfer);
        if (!_memoryCache.TryGetValue(key, out SWTransferConfiguration configuration))
        {
            configuration = await _setting.GetSkywindTransferConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync()
    {
        var key = GetKey(GameProviderConfigurations.WMCasinoTransfer);
        if (!_memoryCache.TryGetValue(key, out WMCasinoTransferConfiguration configuration))
        {
            configuration = await _setting.GetWMCasinoTransferConfigurationAsync().ConfigureAwait(false);

            _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
        }
        return configuration;
    }

    private static string GetKey(string gameProviderCode)
        => $"gameg:gameproviders:setting:{gameProviderCode}".ToLower();
}
