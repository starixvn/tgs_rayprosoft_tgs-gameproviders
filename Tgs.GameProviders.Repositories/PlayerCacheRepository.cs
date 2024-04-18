using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class PlayerCacheRepository(
	IMemoryCache memoryCache,
	IPlayerRepository repository) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:playercache:get:{playerCode}";
		if (!memoryCache.TryGetValue(key, out PlayerInfo? playerModel))
		{
			playerModel = await repository.GetAsync(
				playerCode: playerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)!;

			if (playerModel != null)
			{
				return memoryCache.Set(
					key: key,
					value: playerModel,
					absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
			}
		}

		return playerModel ?? throw new ArgumentNullException(playerCode, "The player does not exist.");
	}
}
