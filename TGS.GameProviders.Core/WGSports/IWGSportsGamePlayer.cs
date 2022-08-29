using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.WGSports;

public interface IWGSportsGamePlayer
{
    /// <summary>
    /// Creates the player asynchronous.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="currencyCode">The currency code.</param>
    /// <returns></returns>
    ValueTask<bool> CreatePlayerAsync(WGSportsConfiguration configuration, string playerCode, string currencyCode);
}
