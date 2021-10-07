using System;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.WGSports
{
    internal class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISettingRepository _settingRepository;

        public WGSportsService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameProviderScheduleRepository gameProviderScheduleRepository,
            IGameRepository gameRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 20m;
            var config = new WGSportsGameProviderScheduleConfig()
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
                GameProviderCode = GameProviderType.WGSports,
                BrandCode = string.Empty,
                AgentId = string.Empty,
                CurrencyCode = string.Empty,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return await GetGameProviderScheduleAsync(gameProviderId).ConfigureAwait(false);
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime)
        {
            var config = JsonSerializer.Deserialize<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);

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
            config.LastGameRecordTime = (lastGameRecorTime.HasValue) ? lastGameRecorTime.Value : config.LastGameRecordTime;
            gameProviderSchedule.Config = JsonSerializer.Serialize(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            var updateResult = await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
            if (updateResult.ModifiedCount > 0)
            {
                return await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        public ClientRoundStatus GetClientRound(decimal playerWinLoss)
           => playerWinLoss switch
           {
               > 0 => ClientRoundStatus.Win,
               <= 0 => ClientRoundStatus.Loss,
           };

        public async ValueTask<GetGame> GetGameByExternalGameId(Guid gameProviderId, string externalGameId)
            => await _gameRepository.GetGameAsync(gameProviderId, externalGameId).ConfigureAwait(false);

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGSports);

        public string GetGameProviderCode()
            => GameProviderType.WGHorse;

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCode(gameProvider.Id, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGSports).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId)
            => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, string.Empty, string.Empty);

        public WGSportsGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => JsonSerializer.Deserialize<WGSportsGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public Task<WGSportsConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<WGSportsConfiguration>(GameProviderConfigurations.WGSports);
    }
}
