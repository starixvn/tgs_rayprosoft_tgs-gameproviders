using TGS.GameProviders.Domains.SettingConfiguration.LY;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record LYConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// Gets or sets the amount maximum.
        /// </summary>
        /// <value>
        /// The amount maximum.
        /// </value>
        public decimal AmountMaximum { get; init; }

        /// <summary>
        /// The amount format
        /// </summary>
        public string AmountFormat { get; init; } = default!;

        /// <summary>
        /// The brand merchants
        /// </summary>
        public LYBrandMerchant[] BrandMerchants { get; init; } = default!;
    }
}
