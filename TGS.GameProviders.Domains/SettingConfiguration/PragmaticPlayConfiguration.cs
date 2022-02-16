namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record PragmaticPlayConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;
        public string ProviderId { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
        public string SecureLogin { get; init; } = default!;
    }
}
