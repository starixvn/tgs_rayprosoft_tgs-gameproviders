using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingService
    {
        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<string> GetGameProviderCurrencyLanguageAsync(Guid gameProviderId, string cultureCode);

        ValueTask<decimal?> GetPlayerBalanceAsync(string playerCode);

        ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync();

        ValueTask<GetGame> GetGameAsync(int gameId);

        ValueTask<Guid> GetBrandIdAsync(string brandCode);

        ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);
    }
}