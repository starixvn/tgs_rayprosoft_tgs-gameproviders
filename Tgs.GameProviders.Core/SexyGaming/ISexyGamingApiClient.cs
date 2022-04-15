using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.SexyGaming.Models;

namespace Tgs.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingApiClient
    {
        ValueTask<SexyGamingCreateMemberResponse> CreatePlayerAsync(Uri apiUrl, SexyGamingCreateMemberRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingGameUrlAsync(Uri apiUrl, SexyGamingGameUrlRequest request);

        ValueTask<SexyGamingGameUrlResponse> GetGamingLoginGameAsync(Uri apiUrl, SexyGamingLoginGameRequest request);

        ValueTask<SexyGamingLogoutResponse> LogoutAsync(Uri apiUrl, SexyGamingLogoutRequest request);
    }
}
