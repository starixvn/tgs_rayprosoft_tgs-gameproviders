using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.AsiaGaming.Models;

namespace TGS.GameProviders.Core.AsiaGaming
{
    public interface IAsiaGamingApiClient
    {
        ValueTask<AsiaGamingCheckOrCreateAccountResponse> CheckOrCreateAccountAsync(
            Uri apiUrl,
            string md5Key,
            string desKey,
            AsiaGamingCheckOrCreateAccountRequest request);

        ValueTask<AsiaGamingCreateGameSessionResponse> CreateGameSessionAsync(
            Uri apiUrl,
            AsiaGamingCreateGameSessionRequest request);
    }
}