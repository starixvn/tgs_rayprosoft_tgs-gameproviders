using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface IWGHorseGamePlayerCreate
    {
        /// <summary>
        /// Creates the player asynchronous.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        ValueTask<bool> CreatePlayerAsync(WGHorseConfiguration configuration, string playerCode, string currencyCode);
    }
}
