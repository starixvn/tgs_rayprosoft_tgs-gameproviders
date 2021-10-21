using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.TCG;

namespace TGS.GameProviders.Core
{
    public interface ITCGGamePlayerCreate
    {
        /// <summary>
        /// Creates the player asynchronous.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <param name="merchantCode">The merchant code.</param>
        /// <param name="desKey">The DES key.</param>
        /// <param name="sha256Key">The sha256 key.</param>
        /// <param name="playerPassword">The player password.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        ValueTask<bool> CreatePlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, string playerPassword, string playerCode);
    }
}
