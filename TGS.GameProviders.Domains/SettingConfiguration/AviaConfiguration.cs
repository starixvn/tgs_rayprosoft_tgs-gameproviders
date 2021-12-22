using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record AviaConfiguration : PluggableConfigurationBase
    {
        public decimal AmountMaximum { get; set; } = default!;

        public string AmountFormat { get; set; } = default!;

        public AviaBrandMerchant[] BrandMerchants { get; set; } = default!;
    }
}
