using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.LY.Models;
using TGS.GameProviders.Core.LY.Models.Commons;

namespace TGS.GameProviders.Core.LY
{
    public interface ILYApiClient
    {
        ValueTask<LYApiResponse<LYFnudResponse>> FundAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYAction action, LYFnudRequest request);

        ValueTask<LYApiResponse<LYFundCheckingResponse>> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYFundCheckingRequest request);

        ValueTask<LYApiResponse<LYGetBalanceResponse>> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYGetBalanceRequest request);

        ValueTask<LYApiResponse<LYGameRecordResponse>> GetGameRecordAsync(Uri apiRecordUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameRecordRequest request);

        ValueTask<LYApiResponse<LYGameUrlResponse>> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, DateTimeOffset dateTimeOffset, LYGameUrlRequest request);

        ValueTask<LYApiResponse<LYKickResponse>> KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, LYKickRequest request);
    }
}
