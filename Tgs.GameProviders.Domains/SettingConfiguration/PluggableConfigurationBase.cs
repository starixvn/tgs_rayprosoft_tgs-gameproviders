namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record PluggableConfigurationBase
    {
        public Uri ServiceEndpointUrl { get; init; } = default!;
    }
}
