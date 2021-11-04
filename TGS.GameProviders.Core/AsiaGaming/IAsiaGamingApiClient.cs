using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.AsiaGaming.Models;

namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingApiClient
    {
        ValueTask<AsiaGamingCheckOrCreateAccountResponse> CheckOrCreateAccountAsync(
            Uri apiUrl,
            string parameter,
            string key,
            AsiaGamingCheckOrCreateAccountRequest request);

        ValueTask<AsiaGamingCreateGameSessionResponse> CreateGameSessionAsync(
            Uri apiUrl,
            string queryString,
            AsiaGamingCreateGameSessionRequest request);
    }
}