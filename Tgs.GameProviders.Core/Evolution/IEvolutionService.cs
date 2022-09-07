using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Evolution;

public interface IEvolutionService
{
    ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

    ValueTask<EvolutionConfiguration> GetSettingConfigurationAsync();
}
