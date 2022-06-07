using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories
{
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
            var key = GameProviderConfigurations.AWC_AELOTTO;
            if (!_memoryCache.TryGetValue(key, out AwcAelottoConfiguration configuration))
            {
                configuration = await _setting.GetAwcAelottoConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_AWS;
            if (!_memoryCache.TryGetValue(key, out AwcAwsConfiguration configuration))
            {
                configuration = await _setting.GetAwcAwsConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_COCKFIGHT;
            if (!_memoryCache.TryGetValue(key, out AwcCockfightConfiguration configuration))
            {
                configuration = await _setting.GetAwcCockfightConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_FACAI;
            if (!_memoryCache.TryGetValue(key, out AwcFacaiConfiguration configuration))
            {
                configuration = await _setting.GetAwcFacaiConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_FS;
            if (!_memoryCache.TryGetValue(key, out AwcFsConfiguration configuration))
            {
                configuration = await _setting.GetAwcFsConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_KM;
            if (!_memoryCache.TryGetValue(key, out AwcKmConfiguration configuration))
            {
                configuration = await _setting.GetAwcKmConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_LUDO;
            if (!_memoryCache.TryGetValue(key, out AwcLudoConfiguration configuration))
            {
                configuration = await _setting.GetAwcLudoConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_PGS;
            if (!_memoryCache.TryGetValue(key, out AwcPgsConfiguration configuration))
            {
                configuration = await _setting.GetAwcPgsConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_RCB;
            if (!_memoryCache.TryGetValue(key, out AwcRcbConfiguration configuration))
            {
                configuration = await _setting.GetAwcRcbConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_SPADE;
            if (!_memoryCache.TryGetValue(key, out AwcSpadeConfiguration configuration))
            {
                configuration = await _setting.GetAwcSpadeConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_VENUS;
            if (!_memoryCache.TryGetValue(key, out AwcVenusConfiguration configuration))
            {
                configuration = await _setting.GetAwcVenusConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public async ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync()
        {
            var key = GameProviderConfigurations.AWC_YL;
            if (!_memoryCache.TryGetValue(key, out AwcYlConfiguration configuration))
            {
                configuration = await _setting.GetAwcYlConfigurationAsync().ConfigureAwait(false);

                _ = _memoryCache.Set(key, configuration, _absoluteExpiration);
            }
            return configuration;
        }

        public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
            => _setting.GetConfigurationAsync<TConfiguration>(key);
    }
}
