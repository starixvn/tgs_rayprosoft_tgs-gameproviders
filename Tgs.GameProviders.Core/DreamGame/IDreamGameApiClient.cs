using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.DreamGame.Models;

namespace Tgs.GameProviders.Core.DreamGame;

public interface IDreamGameApiClient
{
    ValueTask<string> GetAgentFixAsync(Uri apiUrl, string agentName);

    string GetGameResultAsync(Uri videoGameApiUrl, string agentName, string language, string agentFix, string id);

    ValueTask<IEnumerable<GameReportResult>> GetReportAsync(Uri apiUrl, string apiKey, string agentName);

    ValueTask<LoginResult> LoginAsync(Uri apiUrl, string apiKey, string agentName, string username, string password, PlatformType platformType = PlatformType.Desktop, string lang = "en");

    ValueTask<bool> MarkReportAsync(Uri apiUrl, string apiKey, string agentName, IEnumerable<string> externalRoundIds);

    ValueTask<bool> OfflineAsync(Uri apiUrl, string apiKey, string agentName, long memberId);

    ValueTask<IEnumerable<OnlineReportResult>> OnlineReportAsync(Uri apiUrl, string apiKey, string agentName);

    ValueTask<int> SignUpAsync(Uri videoGameApiUrl, string apiKey, string agentName, string username, string password, string currencyNameint, string tableLimit = "A", int winLimit = 0);
}
