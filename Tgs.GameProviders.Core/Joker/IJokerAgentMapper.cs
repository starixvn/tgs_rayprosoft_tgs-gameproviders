using System.Collections.Generic;
using Tgs.GameProviders.Domains.SettingConfiguration.Joker;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerAgentMapper
{
    JokerAgent GetAgent(IEnumerable<JokerBrandMerchant> merchants, string brandCode, string currencyCode);
}