using System;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SBO;

public interface ISBOService
{
    ValueTask<GameInfo> GetGameAsync(int value);

    ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<SBOConfiguration> GetSettingConfigurationAsync();
}
