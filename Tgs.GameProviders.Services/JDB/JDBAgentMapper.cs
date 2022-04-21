using System.Collections.Generic;
using System.Linq;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Domains.SettingConfiguration.JDB;

namespace Tgs.GameProviders.Services.JDB
{
    internal class JDBAgentMapper : IJDBAgentMapper
    {
        public JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode)
            => agents?
                .FirstOrDefault(x => x.CurrencyCode == currencyCode);
    }
}
