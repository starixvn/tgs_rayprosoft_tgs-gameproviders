using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.VR;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.VR
{
    public interface IVRService
    {
        ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode);

        RoundClientStatus GetClientRound(int state);

        ValueTask<Guid> GetGameProviderIdAsync();

        RoundStatus GetRoundStatus(int state);
    }
}
