using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface IWGHorseService
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

        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        ValueTask<GetGame> GetGameByExternalGameId(Guid gameProviderId, string externalGameId);

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
        WGHorseGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        Task<WGHorseConfiguration> GetSettingConfigurationAsync();
    }
}
