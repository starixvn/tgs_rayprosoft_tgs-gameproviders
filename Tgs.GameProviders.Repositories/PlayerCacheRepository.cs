using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class PlayerCacheRepository : IPlayerRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPlayerRepository _repository;

    public PlayerCacheRepository(
        IMemoryCache memoryCache,
        IPlayerRepository repository)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async ValueTask<PlayerInfo> GetAsync(
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:playercache:get:{playerCode}";
        if (!_memoryCache.TryGetValue(key, out PlayerInfo? playerModel))
        {
            playerModel = await _repository.GetAsync(playerCode).ConfigureAwait(false)!;

            if (playerModel != null)
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: playerModel,
                        absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
            }
        }
        return playerModel!;
    }
}
