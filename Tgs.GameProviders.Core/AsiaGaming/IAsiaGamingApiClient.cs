using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.AsiaGaming.Models;

namespace Tgs.GameProviders.Core.AsiaGaming;

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
