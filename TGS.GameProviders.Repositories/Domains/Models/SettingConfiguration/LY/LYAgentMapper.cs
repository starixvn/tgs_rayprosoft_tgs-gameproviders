namespace TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.LY
{
    public record LYAgentMapper
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
