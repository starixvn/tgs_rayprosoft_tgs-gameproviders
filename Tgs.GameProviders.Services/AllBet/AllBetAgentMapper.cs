using System.Linq;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Services.AllBet;

internal class AllBetAgentMapper : IAllBetAgentMapper
{
    public AllBetAgent GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode)
    {
        if (merchants == null || merchants.Length == 0)
        {
            return null;
        }

        var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                       merchants.FirstOrDefault(m => m.BrandCode == CustomConst.BrandCodeDefault);

        return merchant?.Agents.FirstOrDefault(a => a.CurrencyCode == currencyCode);
    }
}
