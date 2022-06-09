namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record PragmaticPlayTransferConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;
        public string ProviderId { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
        public string SecureLogin { get; init; } = default!;
    }
}
