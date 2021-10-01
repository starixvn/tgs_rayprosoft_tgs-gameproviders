using System;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record PluggableConfigurationBase
    {
        public Uri ServiceEndpointUrl { get; set; }
    }
}
