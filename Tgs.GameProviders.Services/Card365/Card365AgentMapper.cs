using System;
using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.Card365;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.Card365;

namespace Tgs.GameProviders.Services.Card365;

internal class Card365AgentMapper : ICard365AgentMapper
{
    public Card365BrandAgent GetAgent(IEnumerable<Card365BrandAgent> brandAgents, string brandCode)
    {
        var agents = brandAgents?.ToArray() ?? Array.Empty<Card365BrandAgent>();

        if (agents.Length == 0)
        {
            throw new ArgumentException("Brand Agent setting is empty");
        }

        return agents.FirstOrDefault(ag => ag.BrandCode == brandCode) ??
               agents.FirstOrDefault(ag => ag.BrandCode == CustomConst.BrandCodeDefault);
    }
}
