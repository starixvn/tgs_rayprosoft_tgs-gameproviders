// ReSharper disable InconsistentNaming

using TGS.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record WMCasinoConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;

        public WMCasinoBrandMerchant[] BrandMerchants { get; init; } = default!;
    }
}
