namespace Tgs.GameProviders.Domains.SettingConfiguration
{
    public record WGSportsConfiguration : PluggableConfigurationBase
    {
        /// <summary>
        /// The amount maximum
        /// </summary>
        public decimal AmountMaximum { get; init; }

        /// <summary>
        /// The amount format
        /// </summary>
        public string AmountFormat { get; init; } = default!;

        public Uri ApiUrl { get; init; } = default!;

        public string PartnerCode { get; init; } = default!;

        public string PartnerKey { get; init; } = default!;

        public string IV { get; init; } = default!;
    }
}
