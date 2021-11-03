using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ISBOService
    {
        ValueTask<BrandGameProvider> GetBrandGameProviderAsync(Guid brandId, Guid gameProviderId);

        ValueTask<GetGame> GetGameAsync(int value);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<GetPlayer> GetPlayerAsync(string playerCode);

        ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
    }
}