using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.ApiClients.KY.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.ApiClients
{
    public interface IKYApiClient
    {
        ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYAction action, KYFnudRequest request);

        ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(KYBrandMerchant brandMerchant, string agentId, KYFundCheckingRequest request);

        ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(KYBrandMerchant brandMerchant, string agentId, KYGetBalanceRequest request);

        ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request);

        ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(KYBrandMerchant brandMerchant, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request);

        ValueTask<KYApiResponse<KYKickResponse>> KickAsync(KYBrandMerchant brandMerchant, string agentId, KYKickRequest request);
    }
}
