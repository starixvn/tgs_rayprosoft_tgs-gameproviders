using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Domains.SettingConfiguration.AllBet;

namespace TGS.GameProviders.Services.AllBet;

internal class AllBetAgentMapper : IAllBetAgentMapper
{
    private static readonly string _DefaultCode = "Default";

    public AllBetAgent GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode)
    {
        if (merchants == null || merchants.Length == 0)
        {
            return null;
        }

        var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                       merchants.FirstOrDefault(m => m.BrandCode == _DefaultCode);

        return merchant?.Agents.FirstOrDefault(a => a.CurrencyCode == currencyCode);
    }
}