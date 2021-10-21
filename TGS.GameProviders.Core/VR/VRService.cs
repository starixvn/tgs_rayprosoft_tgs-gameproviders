using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.Extensions;
using TGS.GameProviders.MongoRepositories;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.VR;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.VR
{
    internal class VRService : IVRService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly ISettingRepository _settingRepository;

        public VRService(
            ISettingRepository settingRepository,
            IGameProviderScheduleRepository gameProviderScheduleRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string brandCode, string merchantid)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var intervalMinuteMax = 24 * 60m;
            var intervalMinute = 20m;
            var config = new VRGameProviderScheduleConfig()
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
                GameProviderCode = GameProviderType.VR,
                BrandCode = brandCode,
                AgentId = merchantid,
                CurrencyCode = string.Empty,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime)
        {
            var config = JsonSerializer.Deserialize<VRGameProviderScheduleConfig>(gameProviderSchedule.Config);
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
                return await GetGameProviderScheduleAsync(gameProviderSchedule.GameProviderId, gameProviderSchedule.BrandCode, gameProviderSchedule.AgentId).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        public async ValueTask<VRBrandMerchant> GetBrandMerchantAsync(string brandCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR).ConfigureAwait(false);
            var brandMerchant = configuration.BrandMerchants
                 .Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants
                    .Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            return brandMerchant;
        }

        public ClientRoundStatus GetClientRound(int state) =>
           state switch
           {
               1 => ClientRoundStatus.Void,
               2 => ClientRoundStatus.Loss,
               3 => ClientRoundStatus.Win,
               _ => ClientRoundStatus.Open
           };

        public Task<GameProvider> GetGameProviderAsync()
                    => _gameProviderRepository.GetGameProviderAsync(GameProviderType.VR);

        public string GetGameProviderCode()
            => GameProviderType.VR;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.VR).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string brandCode, string merchantid)
            => _gameProviderScheduleRepository.GetAsync(gameProviderId, brandCode, merchantid, string.Empty);

        public VRGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => JsonSerializer.Deserialize<VRGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public RoundStatus GetRoundStatus(int state) =>
           state switch
           {
               1 => RoundStatus.Closed,
               2 => RoundStatus.Closed,
               3 => RoundStatus.Closed,
               _ => RoundStatus.Open
           };

        public Task<VRConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<VRConfiguration>(GameProviderConfigurations.VR);
    }
}
