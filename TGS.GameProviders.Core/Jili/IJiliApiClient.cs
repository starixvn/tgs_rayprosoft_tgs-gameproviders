using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Jili.Models;

namespace Tgs.GameProviders.Core.Jili;

public interface IJiliApiClient
{
    ValueTask<JiliGetGameDetailUrlResponse> GetGameDetailUrlAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        string language,
        string externalRoundId);

    ValueTask<JiliKickMemberResponse> KickMemberAsync(
        Uri apiUrl,
        string jiliPlayerId,
        string agentId,
        string agentKey);

    ValueTask<JiliLoginWithoutRedirectResponse> LoginWithoutRedirectAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        JiliLoginWithoutRedirectRequest request);
}
