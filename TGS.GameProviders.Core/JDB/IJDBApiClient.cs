using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.JDB.Models;

namespace TGS.GameProviders.Core.JDB;

public interface IJDBApiClient
{
    ValueTask<JDBBetHistoryResponse> GetBetHistoryUrlAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBBetHistoryRequest request);

    ValueTask<JDBGetTokenResponse> GetTokenAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBGetTokenRequest request);

    ValueTask<JDBLogoutResponse> LogoutAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBLogoutRequest request);
}
