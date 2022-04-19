#nullable enable

using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerAgentMapper : IJokerAgentMapper
{
    private static readonly string _DefaultCode = "Default";

    public JokerAgent? GetAgent(IEnumerable<JokerBrandMerchant> merchants, string brandCode, string currencyCode)
    {
        if (!merchants.Any())
        {
            return null;
        }

        var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                       merchants.FirstOrDefault(m => m.BrandCode == _DefaultCode);

        return merchant?.Agents.FirstOrDefault(a
            => a.CurrencyCode == currencyCode
               && a.Status);
    }
}