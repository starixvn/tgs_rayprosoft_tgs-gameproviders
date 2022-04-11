using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.BigGaming;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.BigGaming
{
    public interface IBigGamingService
    {
        ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode);

        RoundClientStatus GetClientRound(decimal winLoss);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<BigGamingConfiguration> GetSettingConfigurationAsync();
    }
}
