using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class SettingCacheRepository(
	IMemoryCache memoryCache,
	ISettingRepository setting) : ISettingRepository
{
	private readonly TimeSpan _absoluteExpiration = TimeSpan.FromMinutes(5);

	public ValueTask<TConfiguration> GetConfigurationAsync<TConfiguration>(string key, CancellationToken cancellationToken = default)
		=> setting.GetConfigurationAsync<TConfiguration>(key: key, cancellationToken: cancellationToken);

	private static string GetKey(string gameProviderCode)
			=> $"gameg:gameproviders:settingcache:{gameProviderCode}";
}
