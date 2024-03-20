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

    public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(
        string key,
        CancellationToken cancellationToken = default)
        => _setting
        .GetConfigurationAsync<TConfiguration>(
            key: key,
            cancellationToken: cancellationToken);

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
