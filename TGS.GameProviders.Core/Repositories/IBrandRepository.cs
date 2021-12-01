#nullable enable

using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Brand;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IBrandRepository
    {
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);

        ValueTask<Guid> GetBrandIdAsync(string brandCode);

        ValueTask<BrandModel?> GetAsync(Guid brandId);
    }
}