using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Core;

public interface IBrandRepository
{
    ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);

    ValueTask<Guid> GetBrandIdAsync(string brandCode);
}
