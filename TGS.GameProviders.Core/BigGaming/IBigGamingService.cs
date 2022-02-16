using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Core.BigGaming
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
