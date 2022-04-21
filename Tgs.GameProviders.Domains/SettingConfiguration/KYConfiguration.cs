using Tgs.GameProviders.Domains.SettingConfiguration.KY;

namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record KYConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The amount maximum
        /// </summary>
        public decimal AmountMaximum { get; init; } = default!;

        /// <summary>
        /// The amount format
        /// </summary>
        public string AmountFormat { get; init; } = default!;

        /// <summary>
        /// The brand merchants
        /// </summary>
        public KYBrandMerchant[] BrandMerchants { get; init; } = default!;
    }
}
