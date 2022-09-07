using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AsiaGaming;

public interface IAsiaGamingGamePlayer
{
    /// <summary>
    /// Check or create the player asynchronous.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="gameProviderCurrencyCode">The game provider currency code.</param>
    /// <returns></returns>
    ValueTask<bool> CheckOrCreateAccountAsync(AsiaGamingConfiguration configuration, string playerCode, string gameProviderCurrencyCode);
}
