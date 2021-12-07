using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.AllBet.Models;

namespace TGS.GameProviders.Core.AllBet;

public interface IAllBetApiClient
{
    ValueTask<AllBetLoginResponse> LoginAsync(
        Uri apiUrl, string propertyId, string apiKey,
        AllBetLoginRequest request);

    ValueTask<AllBetCheckOrCreateMemberResponse> CheckOrCreateMemberAsync(
        Uri apiUrl, string propertyId,
        string apiKey, AllBetCheckOrCreateMemberRequest request);
}