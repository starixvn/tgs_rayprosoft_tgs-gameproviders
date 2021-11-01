using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Models;

namespace TGS.GameProviders.Core.LY
{
    internal class LYGameProviderScheduleService : ILYGameProviderScheduleService
    {
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;

        public LYGameProviderScheduleService(IGameProviderScheduleRepository gameProviderScheduleRepository)
        {
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 40m;
            var intervalMinute = 10m;
            var config = new LYGameProviderScheduleConfig()
            {
                IntervalMinuteMax = intervalMinuteMax,
                IntervalMinute = intervalMinute,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+8)),
                StartTime = sysTime.AddSeconds((double)intervalMinute * 60 * -1),
                EndTime = sysTime
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderType.LY,
                BrandCode = brandCode,
                AgentId = agentId,
                CurrencyCode = currencyCode,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime)
        {
            var config = GetGameProviderScheduleConfig(gameProviderSchedule);

            if (config.EndTime > sysTime || (sysTime - config.EndTime).TotalSeconds < (double)config.IntervalMinute * 60)
            {
                config.StartTime = sysTime.AddSeconds((double)config.IntervalMinute * 60 * -1);
                config.EndTime = sysTime;
            }
            else
            {
                var endTime = config.EndTime.AddSeconds((double)config.IntervalMinute * 60);
                config.StartTime = config.EndTime;
                config.EndTime = (endTime > sysTime) ? sysTime : endTime;
            }

            config.LastGameRecordTime = lastGameRecorTime ?? config.LastGameRecordTime;
            gameProviderSchedule.Config = JsonSerializer.Serialize(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            return updateResult.ModifiedCount > 0
                ? await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId, gameProviderSchedule.BrandCode, gameProviderSchedule.AgentId, gameProviderSchedule.CurrencyCode).ConfigureAwait(false)
                : null;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode)
           => _gameProviderScheduleRepository.GetAsync(gameProviderId, brandCode, agentId, currencyCode);

        public LYGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
              => JsonSerializer.Deserialize<LYGameProviderScheduleConfig>(gameProviderSchedule.Config);
    }
}
