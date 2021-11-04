using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly TgsContext _context;

        public GameProviderRepository(
            TgsContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async ValueTask<(bool IsCultureCode, string? GameProviderCultureCode)> GetGameProviderCultureCodeAsync(
            Guid gameProviderId, string cultureCode)
        {
            var gameProviderLanguage = await _context.GameProviderLanguages
                .FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.CultureCode == cultureCode)
                .ConfigureAwait(false);

            return gameProviderLanguage == null
                ? new(false, null)
                : new(true, gameProviderLanguage.GameProviderCultureCode);
        }

        public async ValueTask<(bool IsCurrencyCode, string? GameProviderCurrencyCode)> GetGameProviderCurrencyCodeAsync(
            Guid gameProviderId, string currencyCode)
        {
            var gameProviderCurrency = await _context.GameProviderCurrencies
                .FirstOrDefaultAsync(x => x.GameProviderId == gameProviderId && x.CurrencyCode == currencyCode)
                .ConfigureAwait(false);

            return gameProviderCurrency == null
                ? new(false, null)
                : new(true, gameProviderCurrency.GameProviderCurrencyCode);
        }

        public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        {
            var data = await _context.GameProviders.FirstOrDefaultAsync(x => x.Code == gameProviderCode).ConfigureAwait(false);
            return data.Id;
        }

        public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
        {
            var data = await _context.GameProviderConfigs.FirstOrDefaultAsync(gpc
                => gpc.Id == gameProviderConfigurationId).ConfigureAwait(false);

            return (data.DesktopEndpoint, data.MobileEndpoint);
        }
    }
}