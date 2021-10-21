using System;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.SexyGaming;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
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
