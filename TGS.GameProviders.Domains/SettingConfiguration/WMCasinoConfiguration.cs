// ReSharper disable InconsistentNaming

using System;
using TGS.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record WMCasinoConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public WMCasinoBrandMerchant[] BrandMerchants { get; init; }
    }
}
