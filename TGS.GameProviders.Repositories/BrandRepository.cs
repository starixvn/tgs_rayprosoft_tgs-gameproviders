using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Brand;
using TGS.GameProviders.Domains;
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

        public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid gameProviderId, Guid brandId)
        {
            var data = await _context.BrandGameProviders
                .FirstAsync(x => x.GameProviderId == gameProviderId && x.BrandId == brandId)
                .ConfigureAwait(false);

            return data.GameProviderConfigurationId;
        }

        public async ValueTask<Guid> GetBrandIdAsync(string brandCode)
        {
            var brand = await _context.Brands
                .FirstAsync(b => b.Code == brandCode)
                .ConfigureAwait(false);

            return brand.Id;
        }

        public async ValueTask<BrandModel?> GetAsync(Guid brandId)
            => await _context.Brands
                .Where(b => b.Id == brandId)
                .Select(b => new BrandModel(
                    b.Id,
                    b.Code,
                    b.Name,
                    (BrandStatus)b.Status,
                    b.Description,
                    b.LicenseeId))
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
    }
}