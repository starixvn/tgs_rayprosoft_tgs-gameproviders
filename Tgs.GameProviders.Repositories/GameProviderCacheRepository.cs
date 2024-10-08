using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class GameProviderCacheRepository(
	IMemoryCache memoryCache,
	IGameProviderRepository repository) : IGameProviderRepository
{
	public ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId, CancellationToken cancellationToken = default)
		 => repository
		.GetAsync(
			 gameProviderId: gameProviderId,
			 cancellationToken: cancellationToken);

	public ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
		=> repository
		.GetGameEndpointsAsync(
			gameProviderConfigurationId: gameProviderConfigurationId,
			cancellationToken: cancellationToken);

	public async ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:culturecode:{gameProviderId}:{cultureCode}";
		if (!memoryCache.TryGetValue(key, out string? gameProviderCultureCode))
		{
			gameProviderCultureCode = await repository.GetGameProviderCultureCodeAsync(
				gameProviderId: gameProviderId,
				cultureCode: cultureCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
			{
				return memoryCache.Set(
					key: key,
					value: gameProviderCultureCode,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
			}
		}

		return gameProviderCultureCode;
	}

	public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:currencycode:{gameProviderId}:{currencyCode}";
		if (!memoryCache.TryGetValue(key, out string? gameProviderCurrencyCode))
		{
			gameProviderCurrencyCode = await repository.GetGameProviderCurrencyCodeAsync(
				gameProviderId: gameProviderId,
				currencyCode: currencyCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
			{
				return memoryCache.Set(
					key: key,
					value: gameProviderCurrencyCode,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
			}
		}

		return gameProviderCurrencyCode
			?? throw new Exception($"This game product currency does not exist.({currencyCode})");
	}

	public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:gameproviderid:{gameProviderCode}";
		if (!memoryCache.TryGetValue(key, out Guid? gameProviderId))
		{
			gameProviderId = await repository.GetGameProviderIdAsync(
				gameProviderCode: gameProviderCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameProviderId.HasValue)
			{
				return memoryCache.Set(
					key: key,
					value: gameProviderId.Value);
			}
		}

		return gameProviderId
			?? throw new Exception($"Game Provider does not exist.({gameProviderCode})");
	}

	public async ValueTask<SabaSWConfiguration> GetSabaSWConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out SabaSWConfiguration? configurationData))
		{
			configurationData = await repository.GetSabaSWConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache.Set(
				key: key,
				value: configurationData,
				absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}
	public async ValueTask<SabaPlusConfiguration> GetSabaPlusConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out SabaPlusConfiguration? configurationData))
		{
			configurationData = await repository.GetSabaPlusConfigurationAsync(gameProviderConfigurationId, cancellationToken).ConfigureAwait(false);

			return memoryCache.Set(
				key: key,
				value: configurationData,
				absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}
}
