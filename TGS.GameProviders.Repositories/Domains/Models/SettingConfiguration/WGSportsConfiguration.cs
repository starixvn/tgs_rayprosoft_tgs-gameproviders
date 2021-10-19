using System;

namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration
{
    public record WGSportsConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        ///     The amount maximum
        /// </summary>
        public decimal AmountMaximum { get; init; }

        /// <summary>
        ///     The amount format
        /// </summary>
        public string AmountFormat { get; init; }

        public Uri ApiUrl { get; init; }

        public string PartnerCode { get; init; }

        public string PartnerKey { get; init; }

        public string IV { get; init; }
    }
}
