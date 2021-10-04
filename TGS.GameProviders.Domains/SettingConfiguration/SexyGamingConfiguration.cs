using System;
using TGS.GameProviders.Domains.SettingConfiguration.SexyGaming;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record SexyGamingConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string AgentId { get; init; }
        public string Cert { get; init; }
        public SexyGamingBetLimit BetLimit { get; init; }
        public OppositeBetSetting[] OppositeBets { get; init; }
    }
}
