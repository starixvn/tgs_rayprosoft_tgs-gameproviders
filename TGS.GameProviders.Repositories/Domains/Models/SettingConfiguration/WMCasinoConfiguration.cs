// ReSharper disable InconsistentNaming

using System;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    public record WMCasinoConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; }

        public WMCasinoBrandMerchant[] BrandMerchants { get; init; }
    }
}