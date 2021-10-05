using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IWGSportsService
    {
        /// <summary>
        /// Games the provider schedule initialize.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId);

        /// <summary>
        /// Games the provider schedule update asynchronous.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <param name="lastGameRecorTime">The last game recor time.</param>
        /// <param name="sysTime">The system time.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        Task<Guid> GetGameProviderIdAsync();

        /// <summary>
        /// Gets the game provider schedule.
        /// </summary>
        /// <param name="gameProviderId">The game provider identifier.</param>
        /// <returns></returns>
        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId);

        /// <summary>
        /// Gets the game provider schedule configuration.
        /// </summary>
        /// <param name="gameProviderSchedule">The game provider schedule.</param>
        /// <returns></returns>
        WGSportsGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        Task<WGSportsConfiguration> GetSettingConfigurationAsync();

        ValueTask<Game> GetGameByExternalGameId(Guid gameProviderId, string externalGameId);

        ClientRoundStatus GetClientRound(decimal playerWinLoss);
    }
}
