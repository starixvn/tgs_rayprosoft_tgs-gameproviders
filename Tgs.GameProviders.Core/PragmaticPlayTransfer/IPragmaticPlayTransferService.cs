#nullable enable

using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer;

public interface IPragmaticPlayTransferService
{
    ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync();

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PragmaticPlayTransferConfiguration?> GetSettingConfigurationAsync();
}
