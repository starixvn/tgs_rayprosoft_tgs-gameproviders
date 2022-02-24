using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.VR;

namespace TGS.GameProviders.Core.VR
{
    public interface IVRService
    {
        ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode);

        RoundClientStatus GetClientRound(int state);

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        RoundStatus GetRoundStatus(int state);

        ValueTask<VRConfiguration> GetSettingConfigurationAsync();
    }
}
