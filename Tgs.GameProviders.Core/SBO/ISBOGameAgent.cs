using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SBO;

public interface ISBOGameAgent
{
    /// <summary>
    /// Creates the agent asynchronous.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="agentId">The agent id.</param>
    /// <param name="currencyCode">The player currency code. (tgs internal currency code)</param>
    /// <returns></returns>
    ValueTask<bool> CreateAgentAsync(SBOConfiguration configuration, string agentId, string currencyCode);
}
