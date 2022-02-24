using System;
using System.Collections.Generic;
using System.Linq;
using TGS.GameProviders.Core.Card365;
using TGS.GameProviders.Domains.SettingConfiguration.Card365;

namespace TGS.GameProviders.Services.Card365;

internal class Card365AgentMapper : ICard365AgentMapper
{
    private static readonly string _DefaultCode = "Default";

    public Card365BrandAgent GetAgent(IEnumerable<Card365BrandAgent> brandAgents, string brandCode)
    {
        var agents = brandAgents?.ToArray() ?? Array.Empty<Card365BrandAgent>();

        if (agents.Length == 0)
        {
            throw new ArgumentException("Brand Agent setting is empty");
        }

        return agents.FirstOrDefault(ag => ag.BrandCode == brandCode) ??
               agents.FirstOrDefault(ag => ag.BrandCode == _DefaultCode);
    }
}
