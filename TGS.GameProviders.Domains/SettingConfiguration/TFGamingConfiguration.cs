using TGS.GameProviders.Domains.SettingConfiguration.TF;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record TFGamingConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The brand merchants
        /// </summary>
        public TFBrandMerchant[] BrandMerchants { get; init; } = default!;
    }
}