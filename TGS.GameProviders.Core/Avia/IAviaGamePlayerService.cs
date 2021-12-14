using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaGamePlayerService
    {
        /// <summary>
        /// Creates the player asynchronous.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="playerCode">The player code.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <returns></returns>
        ValueTask<bool> CreatePlayerAsync(AviaConfiguration configuration, string playerCode, string currencyCode);
    }
}
