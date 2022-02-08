using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Habanero;

public interface IHabaneroService
{
    ValueTask<GameInfo> GetGameAsync(int gameId);

    ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType);

    ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId);

    ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<PlayerInfo> GetPlayerAsync(string playerCode);

    ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync();
}
