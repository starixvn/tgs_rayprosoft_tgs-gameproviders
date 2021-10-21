using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    /// <summary>
    /// BG 設定檔案
    /// </summary>
    public record BigGamingConfiguration : PluggableConfigurationBase
    {
        public BigGamingBrandMerchant[] BrandMerchants { get; set; }
        public OppositeBetSetting[] OppositeBets { get; init; }
    }
}
