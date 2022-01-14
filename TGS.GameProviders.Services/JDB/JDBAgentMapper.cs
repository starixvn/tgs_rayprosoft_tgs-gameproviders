using System.Collections.Generic;
using System.Linq;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Domains.SettingConfiguration.JDB;

namespace TGS.GameProviders.Services.JDB
{
    internal class JDBAgentMapper : IJDBAgentMapper
    {
        public JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode)
            => agents?
                .FirstOrDefault(x => x.CurrencyCode == currencyCode);
    }
}