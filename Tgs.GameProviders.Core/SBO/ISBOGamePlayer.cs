using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SBO
{
    public interface ISBOGamePlayer
    {
        /// <summary>
        /// Creates the player asynchronous.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="currencyCode">The player currency code.</param>
        /// <returns></returns>
        ValueTask<bool> CreatePlayerAsync(SBOConfiguration configuration, string brandCode, string playerCode,
            string currencyCode);
    }
}
