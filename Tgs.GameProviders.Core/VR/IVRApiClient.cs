using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.VR.Models;
using Tgs.GameProviders.Core.VR.Models.Commons;

namespace Tgs.GameProviders.Core.VR
{
    public interface IVRApiClient
    {
        ValueTask<VRErrorCodeResponse> CreatePlayerAsync(Uri apiUrl, string merchantid, string version, string deskey, VRCreatePlayerRequest source);

        ValueTask<VRFnudResponse> FundAsync(Uri apiUrl, string merchantid, string version, string deskey, VRAction action, VRFnudRequest source);

        ValueTask<VRFundCheckingResponse> FundCheckingAsync(Uri apiUrl, string merchantid, string version, string deskey, VRFundCheckingRequest source);

        ValueTask<VRGetBalanceResponse> GetBalanceAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGetBalanceRequest source);

        ValueTask<VRGameRecordResponse> GetGameRecordAsync(Uri apiUrl, string merchantid, string version, string deskey, VRGameRecordRequest source);

        Uri GetGameURL(Uri apiUrl, string merchantid, string version, string deskey, VRGameUrlRequest source);

        ValueTask<VRErrorCodeResponse> KickAsync(Uri apiUrl, string merchantid, string version, string deskey, VRKickRequest source);
    }
}
