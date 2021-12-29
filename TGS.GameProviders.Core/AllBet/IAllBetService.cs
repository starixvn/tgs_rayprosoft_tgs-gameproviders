using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.AllBet;

public interface IAllBetService
{
    ValueTask<string> GetGameProviderLanguageAsync(string cultureCode);

    ValueTask<AllBetConfiguration> GetSettingConfigurationAsync();

    string ToAllBetPlayerId(string playerCode, string suffix);
}
