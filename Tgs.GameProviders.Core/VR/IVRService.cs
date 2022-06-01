using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.VR;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.VR
{
    public interface IVRService
    {
        ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode);

        GameRoundClientStatus GetClientRound(int state);

        ValueTask<Guid> GetGameProviderIdAsync();

        GameRoundStatus GetRoundStatus(int state);
    }
}
