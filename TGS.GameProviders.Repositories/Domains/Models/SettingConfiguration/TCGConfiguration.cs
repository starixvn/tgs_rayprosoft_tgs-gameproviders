using System;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.TCG;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    /// <summary>
    /// TCG 設定檔案
    /// </summary>
    public record TCGConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The API URL
        /// </summary>
        public Uri ApiUrl { get; init; }
        /// <summary>
        /// The merchants
        /// </summary>
        public TCGMerchant[] Merchants { get; init; }
    }
}
