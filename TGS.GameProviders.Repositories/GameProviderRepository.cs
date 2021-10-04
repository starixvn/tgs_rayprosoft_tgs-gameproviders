using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<GameProviderRepository> _logger;

        public GameProviderRepository(
           ILogger<GameProviderRepository> logger,
           TgsContext context
           )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<GameProvider> GetGameProviderAsync(string gameProviderCode)
            => _context.GameProviders.FirstOrDefaultAsync(x => x.Code == gameProviderCode);

        public Task<GameProvider> GetGameProviderAsync(Guid gameProviderId)
            => _context.GameProviders.FirstOrDefaultAsync(x => x.Id == gameProviderId);

        public async Task<(bool IsCurrencyCode, string GameProviderCurrencyCode)> GetGameProviderCurrencyCode(Guid gameProviderId, string currencyCode)
        {
            var gameProviderCurrency = await _context.GameProviderCurrencies.FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.CurrencyCode == currencyCode).ConfigureAwait(false);
            if (gameProviderCurrency == null)
            {
                return new(false, null);
            }
            return new(true, gameProviderCurrency.GameProviderCurrencyCode);
        }
    }
}
