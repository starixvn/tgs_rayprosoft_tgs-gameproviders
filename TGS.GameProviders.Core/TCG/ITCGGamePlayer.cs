using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Core.TCG
{
    public interface ITCGGamePlayer
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
