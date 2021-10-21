using System;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.WGHorse
{
    internal class WGHorseService : IWGHorseService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISettingRepository _settingRepository;

        public WGHorseService(
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
            var intervalDayMax = 60;
            var intervalDay = 30;
            var sysTime = DateTimeOffset.UtcNow;
            var config = new WGHorseGameProviderScheduleConfig()
            {
                IntervalDayMax = intervalDayMax,
                IntervalDay = intervalDay,
                LastGameRecordTime = new DateTimeOffset(Convert.ToDateTime("1970-01-01"), TimeSpan.FromHours(+0)),
                StartTime = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(intervalDay * -1),
                EndTime = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd")),
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GameProviderType.WGHorse,
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
            var config = JsonSerializer.Deserialize<WGHorseGameProviderScheduleConfig>(gameProviderSchedule.Config);

            if (config.EndTime > sysTime || (sysTime - config.EndTime).TotalDays < (double)config.IntervalDay)
            {
                config.StartTime = sysTime.AddDays((double)config.IntervalDay * -1);
                config.EndTime = sysTime;
            }
            else
            {
                var endTime = config.EndTime.AddDays(1);
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
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGHorse);

        public string GetGameProviderCode()
            => GameProviderType.WGHorse;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProvider.Id, cultureCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : "0";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProvider.Id, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId)
            => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, string.Empty, string.Empty);

        public WGHorseGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => JsonSerializer.Deserialize<WGHorseGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public Task<WGHorseConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<WGHorseConfiguration>(GameProviderConfigurations.WGHorse);
    }
}
