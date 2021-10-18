using System;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients
{
    public interface IEvolutionApiClient
    {
        ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(Uri ua2AuthenticationUrl, EvolutionGameUrlRequest request);

        ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(Uri apiUrl, string casinoID, string gameHistoryApiToken, EvolutionGamHistoryRoundRequest request);
    }
}
