using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.KY;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.KY;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IKYService
    {
        Task<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        Task<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime);

        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<KYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode);

        KYGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        Task<KYConfiguration> GetSettingConfigurationAsync();
    }
}
