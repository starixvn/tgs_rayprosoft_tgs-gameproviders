using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.VR;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    public record VRConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// Gets or sets the amount maximum.
        /// </summary>
        /// <value>
        /// The amount maximum.
        /// </value>
        public decimal AmountMaximum { get; set; }

        /// <summary>
        /// Gets or sets the amount format.
        /// </summary>
        /// <value>
        /// The amount format.
        /// </value>
        public string AmountFormat { get; set; }

        /// <summary>
        /// Gets or sets the brand merchants.
        /// </summary>
        /// <value>
        /// The brand merchants.
        /// </value>
        public VRBrandMerchant[] BrandMerchants { get; set; }
    }
}
