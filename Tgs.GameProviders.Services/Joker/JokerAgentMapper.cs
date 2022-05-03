#nullable enable

using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerAgentMapper : IJokerAgentMapper
{
    public JokerAgent? GetAgentByCurrencyCode(IEnumerable<JokerBrandMerchant> merchants, string brandCode, string currencyCode)
    {
        if (!merchants.Any())
        {
            return null;
        }

        var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                       merchants.FirstOrDefault(m => m.BrandCode == DefConst.BrandCodeDefault);

        return merchant?.Agents.FirstOrDefault(a
            => a.CurrencyCode == currencyCode
               && a.Status);
    }

    public JokerAgent? GetAgentById(IEnumerable<JokerBrandMerchant> merchants, string brandCode, string agentId)
    {
        if (!merchants.Any())
        {
            return null;
        }

        var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                       merchants.FirstOrDefault(m => m.BrandCode == DefConst.BrandCodeDefault);

        return merchant?.Agents.FirstOrDefault(a
            => a.AgentId == agentId
               && a.Status);
    }
}