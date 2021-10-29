using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.SBO.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface ISBOApiClient
    {
        ValueTask<LoginResponse> LoginAsync(Uri apiUrl, LoginRequest request);

        ValueTask<LogoutResponse> LogoutAsync(Uri apiUrl, LogoutRequest request);

        ValueTask<RegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request);

        ValueTask<GetRoundHistoryResponse> GetRoundHistoryAsync(Uri apiUrl, GetRoundHistoryRequest request);

        ValueTask<GetBetHistoryResponse> GetBetHistoryAsync(Uri apiUrl, GetBetHistoryRequest request);

        ValueTask<UpdatePlayerBetLimitsResponse>
            UpdatePlayerBetLimitsAsync(Uri apiUrl, UpdatePlayerBetLimitsRequest request);
    }
}