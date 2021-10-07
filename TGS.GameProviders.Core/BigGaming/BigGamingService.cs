using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.BigGaming
{
    internal class BigGamingService : IBigGamingService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly ISettingRepository _settingRepository;

        public BigGamingService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameProviderScheduleRepository gameProviderScheduleRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string currencyCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 5m;
            var config = new BigGamingGameProviderScheduleConfig()
            {
                IntervalMinuteMax = intervalMinuteMax,
                IntervalMinute = intervalMinute,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+0)),
                StartTime = sysTime.AddSeconds((double)intervalMinute * 60 * -1),
                EndTime = sysTime
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderType.BigGaming,
                BrandCode = string.Empty,
                AgentId = string.Empty,
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
            var config = JsonSerializer.Deserialize<BigGamingGameProviderScheduleConfig>(gameProviderSchedule.Config);

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
                ? await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId, gameProviderSchedule.CurrencyCode).ConfigureAwait(false)
                : null;
        }

        public async ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<BigGamingConfiguration>(GameProviderConfigurations.BigGaming).ConfigureAwait(false);
            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).FirstOrDefault();
        }

        public ClientRoundStatus GetClientRound(decimal winLoss)
            => winLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                < 0 => ClientRoundStatus.Loss,
                0 => ClientRoundStatus.Draw
            };

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.BigGaming);

        public string GetGameProviderCode()
            => GameProviderType.BigGaming;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.BigGaming).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode)
          => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, string.Empty, currencyCode);

        public BigGamingGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => JsonSerializer.Deserialize<BigGamingGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public Task<BigGamingConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<BigGamingConfiguration>(GameProviderConfigurations.BigGaming);
    }
}
