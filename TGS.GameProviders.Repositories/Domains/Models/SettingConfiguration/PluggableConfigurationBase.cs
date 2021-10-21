using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    public record PluggableConfigurationBase
    {
        public Uri ServiceEndpointUrl { get; set; }
    }
}
