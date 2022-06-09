using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.Avia
{
    public interface IAviaService
    {
        ValueTask<AviaBrandMerchant> GetBrandMerchantAsync(string brandCode);

        GameRoundClientStatus GetClientRound(string state);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        GameRoundStatus GetRoundStatus(string state);
    }
}
