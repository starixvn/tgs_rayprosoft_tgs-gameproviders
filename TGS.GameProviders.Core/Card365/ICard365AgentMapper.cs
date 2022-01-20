using System.Collections.Generic;
using TGS.GameProviders.Domains.SettingConfiguration.Card365;

namespace TGS.GameProviders.Core.Card365;

public interface ICard365AgentMapper
{
    public Card365BrandAgent GetAgent(IEnumerable<Card365BrandAgent> brandAgents, string brandCode);
}