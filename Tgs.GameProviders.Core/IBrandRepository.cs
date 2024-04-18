using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core;

public interface IBrandRepository
{
	ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(
		Guid gameProviderId,
		Guid brandId,
		CancellationToken cancellationToken = default);

	ValueTask<Guid> GetBrandIdAsync(
		string brandCode,
		CancellationToken cancellationToken = default);
}
