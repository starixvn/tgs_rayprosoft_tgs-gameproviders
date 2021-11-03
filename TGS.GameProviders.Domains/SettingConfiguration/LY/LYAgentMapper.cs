namespace TGS.GameProviders.Domains.SettingConfiguration.LY
{
    public record LYAgentMapper
    {
        /// <summary>
        /// The currency code
        /// </summary>
        public string CurrencyCode { get; init; } = default!;

        /// <summary>
        /// The agent identifier
        /// </summary>
        public string AgentId { get; init; } = default!;

        /// <summary>
        /// The status
        /// </summary>
        public bool Status { get; init; }
    }
}
