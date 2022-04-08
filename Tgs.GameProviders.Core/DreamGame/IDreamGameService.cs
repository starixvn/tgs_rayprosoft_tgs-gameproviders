using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.DreamGame
{
    public interface IDreamGameService
    {
        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode);

        ValueTask<DreamGameConfiguration> GetSettingConfigurationAsync();
    }
}
