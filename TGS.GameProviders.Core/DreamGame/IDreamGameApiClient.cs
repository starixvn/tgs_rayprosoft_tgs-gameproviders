using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.DreamGame.Models;

namespace TGS.GameProviders.Core.DreamGame
{
    public interface IDreamGameApiClient
    {
        ValueTask<LoginResult> LoginAsync(Uri apiUrl, string apiKey, string agentName, string username, string password, PlatformType platformType = PlatformType.Desktop, string lang = "en");

        ValueTask OfflineAsync(Uri apiUrl, string apiKey, string agentName, string username);

        ValueTask OnlineReportAsync(Uri apiUrl, string apiKey, string agentName);

        ValueTask<int> SignUpAsync(Uri apiUrl, string apiKey, string agentName, string username, string password, string currencyNameint, string tableLimit = "A", int winLimit = 0);
    }
}
