using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.SBO
{
    public interface ISBOService
    {
        ValueTask<GetGame> GetGameAsync(int value);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<GetPlayer> GetPlayerAsync(string playerCode);

        string GetPortfolio(GameType gameType);

        ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
    }
}
