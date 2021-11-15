using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.PragmaticPlay
{
    public interface IPragmaticPlayService
    {
        ValueTask<string> GetGameProviderCurrencyLanguageAsync(string cultureCode);

        ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync();

        ValueTask<GetGame> GetGameAsync(int gameId);
    }
}