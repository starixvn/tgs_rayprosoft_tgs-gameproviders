using System;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record SBOConfiguration : PluggableConfigurationBase
    {
        public string AgentPrefix { get; init; }
        public Uri ApiUrl { get; init; }
        public string CompanyKey { get; init; }
    }
}
