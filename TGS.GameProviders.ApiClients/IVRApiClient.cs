using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.VR.Models;
using TGS.GameProviders.ApiClients.VR.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration.VR;
using TGS.GameProviders.ApiClients.VR;

namespace TGS.GameProviders.ApiClients
{
    public interface IVRApiClient
    {
        ValueTask<VRErrorCodeResponse> CreatePlayerAsync(VRBrandMerchant brandMerchant, VRCreatePlayerRequest request);

        ValueTask<VRFnudResponse> FundAsync(VRBrandMerchant brandMerchant, VRAction action, VRFnudRequest request);

        ValueTask<VRFundCheckingResponse> FundCheckingAsync(VRBrandMerchant brandMerchant, VRFundCheckingRequest request);

        ValueTask<VRGetBalanceResponse> GetBalanceAsync(VRBrandMerchant brandMerchant, VRGetBalanceRequest request);

        ValueTask<VRGameRecordResponse> GetGameRecordAsync(VRBrandMerchant brandMerchant, VRGameRecordRequest request);

        Uri GetGameURLAsync(VRBrandMerchant brandMerchant, VRGameUrlRequest request);

        ValueTask<VRErrorCodeResponse> KickAsync(VRBrandMerchant brandMerchant, VRKickRequest request);
    }
}
