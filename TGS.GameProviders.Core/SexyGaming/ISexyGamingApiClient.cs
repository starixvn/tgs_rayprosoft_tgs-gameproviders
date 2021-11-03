using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.SexyGaming.Models;

namespace TGS.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingApiClient
    {
        ValueTask<SexyGamingCreateMemberResponse> CreatePlayerAsync(Uri apiUrl, SexyGamingCreateMemberRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingGameUrlAsync(Uri apiUrl, SexyGamingGameUrlRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingLoginGameAsync(Uri apiUrl, SexyGamingLoginGameRequest request);

        ValueTask<SexyGamingLogoutResponse> LogoutAsync(Uri apiUrl, SexyGamingLogoutRequest request);
    }
}
