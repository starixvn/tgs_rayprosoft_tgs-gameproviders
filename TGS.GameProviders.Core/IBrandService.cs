using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core
{
    public interface IBrandService
    {
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);

        ValueTask<Guid> GetBrandIdAsync(string brandCode);
    }
}
