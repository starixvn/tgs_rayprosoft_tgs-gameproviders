using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Domains.SettingConfiguration
{
    /// <summary>
    /// BG 設定檔案
    /// </summary>
    public record BigGamingConfiguration : PluggableConfigurationBase
    {
        public BigGamingBrandMerchant[] BrandMerchants { get; init; } = default!;
        public OppositeBetSetting[] OppositeBets { get; init; } = default!;
    }
}
