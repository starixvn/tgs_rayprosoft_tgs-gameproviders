using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Core;

internal class BrandService : IBrandRepository
{
    private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;
    private readonly IMemoryCache _memoryCache;

    public BrandService(
        IGameProvidersQueryServerClient gameProvidersQueryServerClient,
        IMemoryCache memoryCache
        )
    {
        _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId)
    {
        var gameProviderConfigurationId = await _gameProvidersQueryServerClient.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);
        return gameProviderConfigurationId.HasValue
            ? gameProviderConfigurationId.Value
            : throw new Exception($"The gameprovider configuration id does not exist.(gameProviderId={gameProviderId}、brandId{brandId})");
    }

    public async ValueTask<Guid> GetBrandIdAsync(string brandCode)
    {
        if (!_memoryCache.TryGetValue(brandCode, out Guid? brandId))
        {
            brandId = await _gameProvidersQueryServerClient.BrandGetBrandIdAsync(brandCode).ConfigureAwait(false);

            if (brandId.HasValue)
            {
                _memoryCache.Set(brandCode, brandId, TimeSpan.FromHours(1));
                return brandId.Value;
            }
            throw new Exception($"The brand id does not exist.({brandCode})");
        }
        return brandId.Value;
    }
}
