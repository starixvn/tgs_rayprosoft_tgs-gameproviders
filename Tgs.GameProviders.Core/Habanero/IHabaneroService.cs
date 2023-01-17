using System;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.Habanero;

public interface IHabaneroService
{
    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

    ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PlayerInfo> GetPlayerAsync(string playerCode);

    ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync();
}
