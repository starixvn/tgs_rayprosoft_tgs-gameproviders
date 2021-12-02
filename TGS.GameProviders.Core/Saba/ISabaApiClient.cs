using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Saba.Models;

namespace TGS.GameProviders.Core.Saba
{
    public interface ISabaApiClient
    {
        ValueTask<SabaCreateMemberResponse> CreateMemberAsync(
            Uri apiUrl,
            SabaCreateMemberRequest request);

        ValueTask<SabaGetRoundHistoryResponse> GetBetDetailByTransIdAsync(
            Uri apiUrl,
            SabaGetRoundHistoryRequest request);

        ValueTask<SabaGameUrlResponse> GetGameUrlAsync(
                            Uri apiUrl,
            SabaGameUrlRequest request);

        ValueTask<SabaKickUserResponse> KickUserAsync(
            Uri apiUrl,
            SabaKickUserRequest request);
    }
}
