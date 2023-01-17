using System;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.PragmaticPlay;

public interface IPragmaticPlayService
{
    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync();
}
