using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.AllBet.Models;

namespace TGS.GameProviders.Core.AllBet;

public interface IAllBetApiClient
{
    ValueTask<AllBetCheckOrCreateMemberResponse> CheckOrCreateMemberAsync(
        Uri apiUrl,
        string propertyId,
        string apiKey,
        AllBetCheckOrCreateMemberRequest request);

    ValueTask<AllBetLoginResponse> LoginAsync(
            Uri apiUrl,
        string propertyId,
        string apiKey,
        AllBetLoginRequest request);

    ValueTask<AllBetLogoutResponse> LogoutAsync(
        Uri apiUrl,
        string propertyId,
        string apiKey,
        AllBetLogoutRequest request);
}
