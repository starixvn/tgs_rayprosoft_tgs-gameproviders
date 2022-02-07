using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        private readonly TgsContext _context;

        public GameProviderRepository(
            TgsContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        public async ValueTask<GameProviderModel?> GetAsync(string gameProviderCode)
            => await _context.GameProviders
            .Where(x => x.Code == gameProviderCode)
            .Select(x => new GameProviderModel(
                x.Id,
                (GameProviderStatus)x.Status,
                x.Description,
                x.AutoCloseRound,
                x.SupportTestPlayer,
                x.AllowKickPlayer,
                x.PlaceBetTimeout,
                x.SettleBetTimeout,
                x.UniqueId,
                x.TransferWallet,
                x.Code,
                x.Name,
                x.Localization
                ))
           .FirstOrDefaultAsync()
           .ConfigureAwait(false);

        public async ValueTask<GameProviderModel?> GetAsync(Guid gameProviderId)
            => await _context.GameProviders
            .Where(x => x.Id == gameProviderId)
            .Select(x => new GameProviderModel(
                x.Id,
                (GameProviderStatus)x.Status,
                x.Description,
                x.AutoCloseRound,
                x.SupportTestPlayer,
                x.AllowKickPlayer,
                x.PlaceBetTimeout,
                x.SettleBetTimeout,
                x.UniqueId,
                x.TransferWallet,
                x.Code,
                x.Name,
                x.Localization
                ))
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
        {
            var data = await _context.GameProviderConfigs
                .FirstAsync(gpc => gpc.Id == gameProviderConfigurationId)
                .ConfigureAwait(false);

            return (data.DesktopEndpoint, data.MobileEndpoint);
        }

        public async ValueTask<(bool IsCultureCode, string? GameProviderCultureCode)> GetGameProviderCultureCodeAsync(
                    Guid gameProviderId, string cultureCode)
        {
            var gameProviderLanguage = await _context.GameProviderLanguages
                .FirstOrDefaultAsync(x =>
                    x.GameProviderId == gameProviderId &&
                    x.CultureCode == cultureCode)
                .ConfigureAwait(false);

            return gameProviderLanguage == null
                ? new(false, null)
                : new(true, gameProviderLanguage.GameProviderCultureCode);
        }

        public async ValueTask<(bool IsCurrencyCode, string? GameProviderCurrencyCode)> GetGameProviderCurrencyCodeAsync(
            Guid gameProviderId, string currencyCode)
        {
            var gameProviderCurrency = await _context.GameProviderCurrencies
                .FirstOrDefaultAsync(x =>
                    x.GameProviderId == gameProviderId &&
                    x.CurrencyCode == currencyCode)
                .ConfigureAwait(false);

            return gameProviderCurrency == null
                ? new(false, null)
                : new(true, gameProviderCurrency.GameProviderCurrencyCode);
        }

        public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        {
            var data = await _context.GameProviders
                .FirstAsync(x => x.Code == gameProviderCode)
                .ConfigureAwait(false);

            return data.Id;
        }
    }
}