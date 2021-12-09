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
    }
}