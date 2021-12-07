using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Core.SBO
{
    public interface ISBOUtility
    {
        /// <summary>
        /// Get agent id
        /// </summary>
        /// <param name="agentPrefix">The prefix of agent.</param>
        /// <param name="brandCode">The brand code</param>
        /// <param name="currencyCode">The player currency code. (tgs internal currency code)</param>
        /// <returns></returns>
        string GetAgentId(
            string agentPrefix,
            string brandCode,
            string currencyCode);

        /// <summary>
        /// Get game provider portfolio
        /// </summary>
        /// <param name="gameType">The game type.</param>
        /// <returns></returns>
        string GetPortfolio(
            GameType gameType);
    }
}