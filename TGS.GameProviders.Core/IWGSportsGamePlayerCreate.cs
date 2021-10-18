using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface IWGSportsGamePlayerCreate
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
}
