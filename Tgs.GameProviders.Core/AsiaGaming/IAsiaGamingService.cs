using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.AsiaGaming;

public interface IAsiaGamingService
{
    ValueTask<Guid> GetBrandIdAsync(string brandCode);

    ValueTask<GameModel> GetGameAsync(int gameId);

    ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync();
}
