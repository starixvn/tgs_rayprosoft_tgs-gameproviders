using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Domains.SettingConfiguration
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
