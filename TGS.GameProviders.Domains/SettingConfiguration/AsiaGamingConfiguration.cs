using System;
using TGS.GameProviders.Domains.SettingConfiguration.AsiaGaming;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record AsiaGamingConfiguration : PluggableConfigurationBase
    {
        public string AgentCode { get; init; } = default!;

        public string ProductionId { get; init; } = default!;

        public string DesKey { get; init; } = default!;

        public string Md5Key { get; init; } = default!;

        public Uri LoginApiUrl { get; init; } = default!;

        public Uri ApiUrl { get; init; } = default!;

        public OppositeBetSetting[] OppositeBets { get; init; } = default!;

        public string[] LockGameTypes { get; init; } = default!;

        public AsiaGamingGameHistoryType[] GameHistoryTypes { get; init; } = default!;
    }
}
