using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.WGHorse
{
    public interface IWGHorseService
    {
        RoundClientStatus GetClientRound(decimal playerWinLoss);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<WGHorseConfiguration> GetSettingConfigurationAsync();
    }
}
