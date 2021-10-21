using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.SexyGaming.Models;

namespace TGS.GameProviders.ApiClients
{
    public interface ISexyGamingApiClient
    {
        ValueTask<SexyGamingCreateMemberResponse> CreatePlayerAsync(Uri apiUrl, SexyGamingCreateMemberRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingGameUrlAsync(Uri apiUrl, SexyGamingGameUrlRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingLoginGameAsync(Uri apiUrl, SexyGamingLoginGameRequest request);

        ValueTask<SexyGamingLogoutResponse> LogoutAsync(Uri apiUrl, SexyGamingLogoutRequest request);
    }
}
