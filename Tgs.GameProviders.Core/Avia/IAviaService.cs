using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.Avia
{
    public interface IAviaService
    {
        ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode);

        RoundClientStatus GetClientRound(string state);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        RoundStatus GetRoundStatus(string state);
    }
}
