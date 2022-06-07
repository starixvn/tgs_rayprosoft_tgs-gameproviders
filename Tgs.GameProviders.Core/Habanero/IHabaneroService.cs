using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.Habanero;

public interface IHabaneroService
{
    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

    ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PlayerModel> GetPlayerAsync(string playerCode);

    ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync();
}
