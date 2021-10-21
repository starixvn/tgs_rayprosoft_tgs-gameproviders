using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.TCG.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface ITCGApiClient
    {
        ValueTask<TCGKickOutLottoPlayerResponse> KickOutLottoPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGKickOutLottoPlayerRequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameByELOTTORequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameBySEARequest request);

        ValueTask<TCGLaunchGameResponse> LaunchGameAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGLaunchGameByLOTTOVNRequest request);

        ValueTask<TCGRegisterPlayerResponse> RegisterPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGRegisterPlayerRequest request);

        ValueTask<TCGUpdatePasswordPlayerResponse> UpdatePasswordPlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, TCGUpdatePasswordPlayerRequest request);
    }
}
