using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Jili.Models;

namespace TGS.GameProviders.Core.Jili
{
    public interface IJiliApiClient
    {
        ValueTask<JiliLoginWithoutRedirectResponse> LoginWithoutRedirectAsync(
            Uri apiUrl,
            string agentId,
            string agentKey,
            JiliLoginWithoutRedirectRequest request);

        ValueTask<JiliKickMemberResponse> KickMemberAsync(
            Uri apiUrl,
            string jiliPlayerId,
            string agentId,
            string agentKey);
    }
}