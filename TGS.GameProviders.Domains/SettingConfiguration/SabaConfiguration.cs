namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record SabaConfiguration : PluggableConfigurationBase
    {
        public string OperatorId { get; init; } = default!;

        public string VendorId { get; init; } = default!;

        public string SecretKey { get; init; } = default!;

        public Uri ApiUrl { get; init; } = default!;

        public bool IsTestEnvironment { get; init; } = false;
    }
}
