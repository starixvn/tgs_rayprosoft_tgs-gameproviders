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

	public async ValueTask<EvolutionTransferConfiguration> GetEvolutionTransferConfigurationAsync(CancellationToken cancellationToken = default)
	{
		var key = GetKey(GameProviderConfigurations.EvolutionTransfer);
		if (!memoryCache.TryGetValue(key, out EvolutionTransferConfiguration? configuration))
		{
			configuration = await setting
				.GetEvolutionTransferConfigurationAsync(
					cancellationToken: cancellationToken)
				.ConfigureAwait(false)!;

			return memoryCache.Set(
					key,
					configuration,
					_absoluteExpiration);
		}

		return configuration ?? throw new ArgumentNullException(key, "This game product setting does not exist.");
	}

	public async ValueTask<SkywindTransferConfiguration> GetSkywindTransferConfigurationAsync(CancellationToken cancellationToken = default)
	{
		var key = GetKey(GameProviderConfigurations.SkywindTransfer);
		if (!memoryCache.TryGetValue(key, out SkywindTransferConfiguration? configuration))
		{
			configuration = await setting
				.GetSkywindTransferConfigurationAsync(
					cancellationToken: cancellationToken)
				.ConfigureAwait(false)!;

			return memoryCache
				.Set(
					key,
					configuration,
					_absoluteExpiration);
		}

		return configuration ?? throw new ArgumentNullException(key, "This game product setting does not exist.");
	}

	private static string GetKey(string gameProviderCode)
			=> $"gameg:gameproviders:settingcache:{gameProviderCode}";
}
