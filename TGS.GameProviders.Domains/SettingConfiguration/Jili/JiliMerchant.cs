using System.Collections.Generic;

namespace TGS.GameProviders.Domains.SettingConfiguration.Jili
{
    public class JiliMerchant
    {
        public IEnumerable<string> Brands { get; init; } = default!;

        public string AgentId { get; init; } = default!;

        public string AgentKey { get; init; } = default!;
    }
}