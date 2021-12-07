using System;
using TGS.GameProviders.Domains.SettingConfiguration.AllBet;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    public record AllBetConfiguration : PluggableConfigurationBase
    {
        public Uri ApiUrl { get; init; } = default!;

        public AllBetBrandMerchant[] BrandMerchants { get; init; } = default!;
    }
}