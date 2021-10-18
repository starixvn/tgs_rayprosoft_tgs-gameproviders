using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.KY;
using TGS.GameProviders.ApiClients.KY.Models;
using TGS.GameProviders.ApiClients.KY.Models.Commons;

namespace TGS.GameProviders.ApiClients
{
    public interface IKYApiClient
    {
        ValueTask<KYApiResponse<KYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYAction action, KYFnudRequest request);

        ValueTask<KYApiResponse<KYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYFundCheckingRequest request);

        ValueTask<KYApiResponse<KYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYGetBalanceRequest request);

        ValueTask<KYApiResponse<KYGameRecordResponse>> GetGameRecordAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYGameRecordRequest request);

        ValueTask<KYApiResponse<KYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, KYGameUrlRequest request);

        ValueTask<KYApiResponse<KYKickResponse>> KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, KYKickRequest request);
    }
}
