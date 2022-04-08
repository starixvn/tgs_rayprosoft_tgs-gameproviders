namespace Tgs.GameProviders.Domains.SettingConfiguration.LY
{
    public record LYBrandMerchant
    {
        /// <summary>
        /// The brand code
        /// </summary>
        public string BrandCode { get; init; } = default!;

        /// <summary>
        /// The API URL
        /// </summary>
        public Uri ApiUrl { get; init; } = default!;

        /// <summary>
        /// The API record URL
        /// </summary>
        public Uri ApiRecordUrl { get; init; } = default!;

        /// <summary>
        /// The deskey
        /// </summary>
        public string Deskey { get; init; } = default!;

        /// <summary>
        /// The md5key
        /// </summary>
        public string Md5key { get; init; } = default!;

        /// <summary>
        /// The agent mappers
        /// </summary>
        public LYAgentMapper[] AgentMappers { get; init; } = default!;

        /// <summary>
        /// The status
        /// </summary>
        public bool Status { get; init; }
    }
}
