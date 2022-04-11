using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.AllBet;

public interface IAllBetService
{
    ValueTask<string> GetGameProviderLanguageAsync(string cultureCode);

    ValueTask<AllBetConfiguration> GetSettingConfigurationAsync();

    string ToAllBetPlayerId(string playerCode, string suffix);
}
