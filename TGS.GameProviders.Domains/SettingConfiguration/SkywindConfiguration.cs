using System;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record SkywindConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }
        public Uri LiveLobbyUrl { get; init; }
        public string MerchId { get; init; }
        public string MerchPwd { get; init; }
        public string SecretKey { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
    }
}
