using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.DreamGame
{
    public interface IDreamGameService
    {
        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode);

        ValueTask<DreamGameConfiguration> GetSettingConfigurationAsync();
    }
}
