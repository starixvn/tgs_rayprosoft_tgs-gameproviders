using Tgs.GameCommand.Client;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class BrandRepository(
	IGameCommandClient client) : IBrandRepository
{
	public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId, CancellationToken cancellationToken = default)
		=> await client.BrandGetGameProviderConfigurationIdAsync(
			brandId: brandId,
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(new { gameProviderId, brandId }.ToString(), "The gameprovider configuration id does not exist.");

	public async ValueTask<Guid> GetBrandIdAsync(string brandCode, CancellationToken cancellationToken = default)
		=> await client.BrandGetBrandIdAsync(
			brandCode,
			cancellationToken)
		.ConfigureAwait(false)
		?? throw new ArgumentNullException(brandCode, "The brand id does not exist.");
}
