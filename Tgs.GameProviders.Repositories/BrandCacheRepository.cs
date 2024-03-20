using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class BrandCacheRepository : IBrandRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly IBrandRepository _repository;

    public BrandCacheRepository(
        IMemoryCache memoryCache,
        IBrandRepository repository
        )
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(
        Guid gameProviderId,
        Guid brandId,
        CancellationToken cancellationToken = default)
    => _repository
        .GetBrandGameProviderConfigurationIdAsync(
            gameProviderId: gameProviderId,
            brandId: brandId,
            cancellationToken: cancellationToken);

    public async ValueTask<Guid> GetBrandIdAsync(
        string brandCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:gameproviders:brandcache:get:brandid:{brandCode}";
        if (!_memoryCache.TryGetValue(key, out Guid? brandId))
        {
            brandId = await _repository
                .GetBrandIdAsync(
                    brandCode: brandCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)!;

            if (brandId.HasValue)
            {
                return _memoryCache
                    .Set(
                        key: key,
                        value: brandId.Value);
            }
        }

        return brandId
            ?? throw new Exception($"The brand id does not exist.({brandCode})");
    }
}
