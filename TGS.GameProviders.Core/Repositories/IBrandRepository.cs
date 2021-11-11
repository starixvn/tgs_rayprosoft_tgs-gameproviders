using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.Repositories
{
    public interface IBrandRepository
    {
        Task<Guid?> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId);
    }
}
