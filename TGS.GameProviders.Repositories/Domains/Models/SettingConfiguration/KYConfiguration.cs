using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.KY;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    public record KYConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// Gets or sets the amount maximum.
        /// </summary>
        /// <value>
        /// The amount maximum.
        /// </value>
        public decimal AmountMaximum { get; set; }

        /// <summary>
        /// The amount format
        /// </summary>
        public string AmountFormat { get; set; }

        /// <summary>
        /// The brand merchants
        /// </summary>
        public KYBrandMerchant[] BrandMerchants { get; set; }
    }
}
