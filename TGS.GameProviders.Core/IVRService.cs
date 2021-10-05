using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.VR;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.VR;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IVRService
    {
        ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        Task<VRConfiguration> GetSettingConfigurationAsync();

        RoundStatus GetRoundStatus(int state);

        ClientRoundStatus GetClientRound(int state);

        /// <summary>
        /// Gets the game provider schedule asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="merchantid">The merchantid.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string merchantid);

        /// <summary>
        /// Games the provider schedule initialize asynchronous.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="merchantid">The merchantid.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string merchantid);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="responses">The responses.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        VRGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);
    }
}
