using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.VR;

namespace TGS.GameProviders.Core
{
    public interface IVRGamePlayer
    {
        /// <summary>
        /// Creates the player asynchronous.
        /// </summary>
        /// <param name="brandMerchant">The brand merchant.</param>
        /// <param name="playerCode">The player code.</param>
        /// <returns></returns>
        ValueTask<bool> CreatePlayerAsync(VRBrandMerchant brandMerchant, string playerCode);
    }
}
