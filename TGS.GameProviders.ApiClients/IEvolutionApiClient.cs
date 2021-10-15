using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients
{
    public interface IEvolutionApiClient
    {
        ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request);

        ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(EvolutionConfiguration configuration, EvolutionGamHistoryRoundRequest request);
    }
}
