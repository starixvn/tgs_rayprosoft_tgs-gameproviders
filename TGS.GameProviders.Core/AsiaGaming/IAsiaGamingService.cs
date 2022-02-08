using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.AsiaGaming;

public interface IAsiaGamingService
{
    ValueTask<Guid> GetBrandIdAsync(string brandCode);

    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync();
}
