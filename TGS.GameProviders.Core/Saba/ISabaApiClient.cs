using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.Core.Saba
{
    public interface ISabaApiClient
    {
        ValueTask<SabaGameUrlResponse> GetGameUrlAsync(
            Uri apiUrl,
            SabaGameUrlRequest request);

        ValueTask<SabaCreateMemberResponse> CreateMemberAsync(
            Uri apiUrl,
            SabaCreateMemberRequest request);

        ValueTask<SabaGetRoundHistoryResponse> GetBetDetailByTransIdAsync(
            Uri apiUrl,
            SabaGetRoundHistoryRequest request);
    }
}