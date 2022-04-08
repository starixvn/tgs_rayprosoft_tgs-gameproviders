using System.Collections.Generic;
using Tgs.GameProviders.Domains.SettingConfiguration.JDB;

namespace Tgs.GameProviders.Core.JDB;

public interface IJDBAgentMapper
{
    JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode);
}
