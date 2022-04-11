using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Evolution.Models;

namespace Tgs.GameProviders.Core.Evolution
{
    public interface IEvolutionApiClient
    {
        ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(Uri ua2AuthenticationUrl, EvolutionGameUrlRequest request);

        ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(Uri apiUrl, string casinoID, string gameHistoryApiToken, EvolutionGamHistoryRoundRequest request);
    }
}
