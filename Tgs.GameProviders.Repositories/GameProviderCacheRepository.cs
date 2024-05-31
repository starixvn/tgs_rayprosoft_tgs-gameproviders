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

	public async ValueTask<AVIAGamingConfiguration> GetAVIAGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AVIAGamingConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAVIAGamingConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcAelottoConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcAelottoConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcAwsConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcAwsConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcBgConfiguration> GetAwcBgConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcBgConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcBgConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcCockfightConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcCockfightConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcFacaiConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcFacaiConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcFsConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcFsConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcKmConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcKmConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcLudoConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcLudoConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcPgsConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcPgsConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcRcbConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcRcbConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcSgConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcSgConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcSpadeConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcSpadeConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcVenusConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcVenusConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcYesbingoConfiguration> GetAwcYesbingoConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcYesbingoConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcYesbingoConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out AwcYlConfiguration? configurationData))
		{
			configurationData = await repository
				.GetAwcYlConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out BTiSportsConfiguration? configurationData))
		{
			configurationData = await repository
				.GetBTiSportsConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<Cards365TransferConfiguration> GetCards365TransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out Cards365TransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetCards365TransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<CQ9GamingConfiguration> GetCQ9GamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out CQ9GamingConfiguration? configurationData))
		{
			configurationData = await repository
				.GetCQ9GamingConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<CrPatiConfiguration> GetCrPatiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out CrPatiConfiguration? configurationData))
		{
			configurationData = await repository
				.GetCrPatiConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out DreamGamingTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetDreamGamingTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<EndorphinaConfiguration> GetEndorphinaConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out EndorphinaConfiguration? configurationData))
		{
			configurationData = await repository
				.GetEndorphinaConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out FaChaiConfiguration? configurationData))
		{
			configurationData = await repository
				.GetFaChaiConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<FunkyConfiguration> GetFunkyConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out FunkyConfiguration? configurationData))
		{
			configurationData = await repository
				.GetFunkyConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

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
			gameProviderCultureCode = await repository
				.GetGameProviderCultureCodeAsync(
					gameProviderId,
					cultureCode,
					cancellationToken)
				.ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(gameProviderCultureCode))
			{
				return memoryCache
					.Set(
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
			gameProviderCurrencyCode = await repository
				.GetGameProviderCurrencyCodeAsync(
					gameProviderId,
					currencyCode,
					cancellationToken)
				.ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(gameProviderCurrencyCode))
			{
				return memoryCache
					 .Set(
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
			gameProviderId = await repository
				.GetGameProviderIdAsync(
					gameProviderCode: gameProviderCode,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameProviderId.HasValue)
			{
				return memoryCache
					.Set(
						key: key,
						value: gameProviderId.Value);
			}
		}

		return gameProviderId
			?? throw new Exception($"Game Provider does not exist.({gameProviderCode})");
	}

	public async ValueTask<GKXGamingConfiguration> GetGKXGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out GKXGamingConfiguration? configurationData))
		{
			configurationData = await repository.GetGKXGamingConfigurationAsync(
				gameProviderConfigurationId: gameProviderConfigurationId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out HabaneroTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetHabaneroTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<InplayMatrixESportsConfiguration> GetInplayMatrixESportsConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out InplayMatrixESportsConfiguration? configurationData))
		{
			configurationData = await repository
				.GetInplayMatrixESportsConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out JDBTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetJDBTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out JiliTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetJiliTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<JokerConfiguration> GetJokerConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out JokerConfiguration? configurationData))
		{
			configurationData = await repository
				.GetJokerConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<KaiyuanConfiguration> GetKaiyuanConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)

	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out KaiyuanConfiguration? configurationData))
		{
			configurationData = await repository
				.GetKaiyuanConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<LEGamingConfiguration> GetLEGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out LEGamingConfiguration? configurationData))
		{
			configurationData = await repository
				.GetLEGamingConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<OBetConfiguration> GetOBetConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out OBetConfiguration? configurationData))
		{
			configurationData = await repository
				.GetOBetConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<PinnacleConfiguration> GetPinnacleConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out PinnacleConfiguration? configurationData))
		{
			configurationData = await repository
				.GetPinnacleConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out PragmaticPlayTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetPragmaticPlayTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<SabaTransferConfiguration> GetSabaTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out SabaTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetSabaTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out SBOTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetSBOTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<SM01Configuration> GetSM01ConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out SM01Configuration? configurationData))
		{
			configurationData = await repository
				.GetSM01ConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out TFGamingTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetTFGamingTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<TPhereConfiguration> GetTPhereConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out TPhereConfiguration? configurationData))
		{
			configurationData = await repository
				.GetTPhereConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<V8PokerConfiguration> GetV8PokerConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out V8PokerConfiguration? configurationData))
		{
			configurationData = await repository
				.GetV8PokerConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<VRGamingConfiguration> GetVRGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out VRGamingConfiguration? configurationData))
		{
			configurationData = await repository.GetVRGamingConfigurationAsync(
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

	public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out WGFmGamingConfiguration? configurationData))
		{
			configurationData = await repository
				.GetWGFmGamingConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<WGViaGamingConfiguration> GetWGViaGamingConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)

	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out WGViaGamingConfiguration? configurationData))
		{
			configurationData = await repository
				.GetWGViaGamingConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<WicketCricketConfiguration> GetWicketCricketConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out WicketCricketConfiguration? configurationData))
		{
			configurationData = await repository
				.GetWicketCricketConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out WMCasinoTransferConfiguration? configurationData))
		{
			configurationData = await repository
				.GetWMCasinoTransferConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}

	public async ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(Guid gameProviderConfigurationId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gameprovidercache:get:configuration:{gameProviderConfigurationId}";
		if (!memoryCache.TryGetValue(key, out YeeBetConfiguration? configurationData))
		{
			configurationData = await repository
				.GetYeeBetConfigurationAsync(
					gameProviderConfigurationId: gameProviderConfigurationId,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache
				.Set(
					key: key,
					value: configurationData,
					absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(5));
		}

		return configurationData ?? throw new ArgumentNullException(gameProviderConfigurationId.ToString(), $"Game Provider Configuration does not exist");
	}
}
