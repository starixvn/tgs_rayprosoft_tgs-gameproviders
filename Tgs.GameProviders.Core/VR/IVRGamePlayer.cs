using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.VR;

namespace Tgs.GameProviders.Core.VR;

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
