using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class BrandCacheRepository(
	IMemoryCache memoryCache,
	IBrandRepository repository) : IBrandRepository
{
	public ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId, CancellationToken cancellationToken = default)
	=> repository.GetBrandGameProviderConfigurationIdAsync(
		gameProviderId: gameProviderId,
		brandId: brandId,
		cancellationToken: cancellationToken);

	public async ValueTask<Guid> GetBrandIdAsync(string brandCode, CancellationToken cancellationToken = default)
	{
		var key = $"game:gameproviders:brandcache:get:brandid:{brandCode}";
		if (!memoryCache.TryGetValue(key, out Guid? brandId))
		{
			brandId = await repository
				.GetBrandIdAsync(
					brandCode: brandCode,
					cancellationToken: cancellationToken)
				.ConfigureAwait(false)!;

			if (brandId.HasValue)
			{
				return memoryCache
					.Set(
						key: key,
						value: brandId.Value);
			}
		}

		return brandId ?? throw new ArgumentNullException(brandCode, $"The brand id does not exist.");
	}
}
