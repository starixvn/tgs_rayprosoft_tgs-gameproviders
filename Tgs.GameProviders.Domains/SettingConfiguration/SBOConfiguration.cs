namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record SBOConfiguration : PluggableConfigurationBase
    {
        public string AgentPrefix { get; init; } = default!;
        public Uri ApiUrl { get; init; } = default!;
        public string CompanyKey { get; init; } = default!;
    }
}
