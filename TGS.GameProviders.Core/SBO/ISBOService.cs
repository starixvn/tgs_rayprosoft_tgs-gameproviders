using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.SBO;

public interface ISBOService
{
    ValueTask<GameModel> GetGameAsync(int value);

    ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
}
