using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.BigGaming;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IBigGamingService
    {
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string currencyCode);

        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime);

        ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode);

        ClientRoundStatus GetClientRound(decimal winLoss);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode);

        BigGamingGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        Task<BigGamingConfiguration> GetSettingConfigurationAsync();
    }
}
