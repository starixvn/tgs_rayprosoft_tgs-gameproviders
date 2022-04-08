using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.AllBet.Models;

namespace Tgs.GameProviders.Core.AllBet;

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
