#nullable enable

using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IBrandRepository
    {
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);

        ValueTask<Guid> GetBrandIdAsync(string brandCode);
    }
}
