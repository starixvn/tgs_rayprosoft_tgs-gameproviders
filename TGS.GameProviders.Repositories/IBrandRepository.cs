using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    public interface IBrandRepository
    {
        Task<Brand> GetBrandAsync(Guid brandId);

        Task<Guid?> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);
    }
}
