using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class BrandRepository : IBrandRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<BrandRepository> _logger;

        public BrandRepository(
            ILogger<BrandRepository> logger,
            TgsContext context
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid?> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId)
        {
            var data = await _context.BrandGameProviders
                .FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.BrandId == brandId)
                .ConfigureAwait(false);
            return data?.GameProviderConfigurationId;
        }
    }
}
