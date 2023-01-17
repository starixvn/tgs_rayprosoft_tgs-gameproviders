using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class GameCacheRepository : IGameRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IGameRepository _repository;

    public GameCacheRepository(
        IMemoryCache memoryCache,
        IGameRepository repository)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async ValueTask<GameInfo> GetAsync(
        int gameId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:gamecache:get:{gameId}";
        if (!_memoryCache.TryGetValue(key, out GameInfo? gameModel))
        {
            gameModel = await _repository
                .GetAsync(
                    gameId: gameId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (gameModel is not null)
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: gameModel,
                        absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
            }
        }
        return gameModel!;
    }

    public IAsyncEnumerable<GameInfo> GetGameListAsync(
        Guid gameProviderId,
        GameType gameType,
        CancellationToken cancellationToken = default)
        => _repository
        .GetGameListAsync(
            gameProviderId: gameProviderId,
            gameType: gameType,
            cancellationToken: cancellationToken);
}
