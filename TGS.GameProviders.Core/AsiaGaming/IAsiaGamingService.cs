using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingService
    {
        ValueTask<Guid> GetBrandIdAsync(string brandCode);

        ValueTask<GameModel> GetGameAsync(int gameId);

        ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<string> GetGameProviderCurrencyLanguageAsync(Guid gameProviderId, string cultureCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<decimal?> GetPlayerBalanceAsync(string playerCode);

        ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync();
    }
}
