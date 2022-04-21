using Tgs.GameProviders.Domains.SettingConfiguration.JDB;

namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record JDBConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;

        public bool MoreGame { get; init; }

        public string AESIv { get; init; } = default!;

        public string AESKey { get; init; } = default!;

        public string ClientDomain { get; init; } = default!;

        public int TimezoneHour { get; init; }

        public IEnumerable<JDBAgent> Agents { get; init; } = default!;
    }
}
