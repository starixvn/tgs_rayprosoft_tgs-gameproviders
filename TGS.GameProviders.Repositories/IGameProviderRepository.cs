using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderRepository
    {
        Task<GameProvider> GetGameProviderAsync(string gameProviderCode);

        Task<GameProvider> GetGameProviderAsync(Guid gameProviderId);

        Task<(bool IsCurrencyCode, string GameProviderCurrencyCode)> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);
    }
}
