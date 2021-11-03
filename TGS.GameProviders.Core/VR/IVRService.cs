using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.VR;

namespace TGS.GameProviders.Core.VR
{
    public interface IVRService
    {
        ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(int state);

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        RoundStatus GetRoundStatus(int state);

        Task<VRConfiguration> GetSettingConfigurationAsync();
    }
}
