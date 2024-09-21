using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SabaPlus.Domains.FromProviders;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameCacheRepository(
	IMemoryCache memoryCache,
	IGameRepository repository) : IGameRepository
{
	public async ValueTask<GameInfo> GetAsync(int gameId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gamecache:get:{gameId}";
		if (!memoryCache.TryGetValue(key, out GameInfo? gameModel))
		{
			gameModel = await repository.GetAsync(
				gameId: gameId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameModel is not null)
			{
				return memoryCache.Set(
					key: key,
					value: gameModel,
					absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
			}
		}

		return gameModel ?? throw new ArgumentNullException(gameId.ToString(), $"This game does not exist.");
	}

	public async ValueTask<GameInfo> GetByExternalIdAsync(Guid gameProviderId, string externalId, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:gamecache:getbyexternalid:{gameProviderId}:{externalId}";
		if (!memoryCache.TryGetValue(key, out GameInfo? gameModel))
		{
			gameModel = await repository.GetByExternalIdAsync(
				gameProviderId: gameProviderId,
				externalId: externalId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameModel is not null)
			{
				return memoryCache.Set(
					key: key,
					value: gameModel,
					absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
			}
		}

		return gameModel ?? throw new ArgumentNullException($"{gameProviderId} ({externalId})", $"This game does not exist.");
	}

	public IAsyncEnumerable<GameInfo> GetGameListAsync(Guid gameProviderId, GameType gameType, CancellationToken cancellationToken = default)
		=> repository.GetGameListAsync(gameProviderId: gameProviderId, gameType: gameType, cancellationToken: cancellationToken);
}
