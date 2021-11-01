using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.SBO.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface ISBOApiClient
    {
        ValueTask<SBOLoginResponse> LoginAsync(Uri apiUrl, SBOLoginRequest request);

        ValueTask<SBOLogoutResponse> LogoutAsync(Uri apiUrl, SBOLogoutRequest request);

        ValueTask<SBORegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, SBORegisterPlayerRequest request);

        ValueTask<SBOGetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, SBOGetRoundHistoryRequest request);

        ValueTask<SBOGetBetHistoryResponse> GetBetHistoryAsync(Uri apiUrl, SBOGetBetHistoryRequest request);

        ValueTask<SBOUpdatePlayerBetLimitsResponse>
            UpdatePlayerBetLimitsAsync(Uri apiUrl, SBOUpdatePlayerBetLimitsRequest request);
    }
}