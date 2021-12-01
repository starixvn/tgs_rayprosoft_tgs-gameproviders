using System;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record PragmaticPlayConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public string ProviderId { get; init; }
        public string SecretKey { get; init; }
        public string SecureLogin { get; init; }
    }
}
