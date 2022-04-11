using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Card365;

public interface ICard365Service
{
    ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<Card365Configuration> GetSettingConfigurationAsync();
}
