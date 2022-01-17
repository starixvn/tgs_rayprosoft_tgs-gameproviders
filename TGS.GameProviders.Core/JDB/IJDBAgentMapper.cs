using System.Collections.Generic;
using TGS.GameProviders.Domains.SettingConfiguration.JDB;

namespace TGS.GameProviders.Core.JDB;

public interface IJDBAgentMapper
{
    JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode);
}