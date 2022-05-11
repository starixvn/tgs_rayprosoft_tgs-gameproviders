#nullable enable

using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer;

public interface IPragmaticPlayTransferService
{
    ValueTask<GameInfo?> GetGameAsync(int gameId);

    ValueTask<GameProviderInfo?> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PragmaticPlayTransferConfiguration?> GetSettingConfigurationAsync();
}