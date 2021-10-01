namespace TGS.GameProviders.Domains.SettingConfiguration.KY
{
    public record KYAgentMapper
    {
        /// <summary>
        /// The currency code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The agent identifier
        /// </summary>
        public string AgentId { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        public bool Status { get; set; }
    }
}
