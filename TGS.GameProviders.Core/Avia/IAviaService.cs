using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaService
    {
        ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(string state);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        RoundStatus GetRoundStatus(string state);

        Task<AviaConfiguration> GetSettingConfigurationAsync();
    }
}
