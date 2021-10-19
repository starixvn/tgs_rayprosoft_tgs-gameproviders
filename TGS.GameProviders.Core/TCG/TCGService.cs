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
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.TCG;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProviderScheduleRepository _gameProviderScheduleRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISettingRepository _settingRepository;

        public TCGService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IGameProviderScheduleRepository gameProviderScheduleRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _gameProviderScheduleRepository = gameProviderScheduleRepository ?? throw new ArgumentNullException(nameof(gameProviderScheduleRepository));
        }

        public async ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode)
        {
            var sysTime = DateTimeOffset.UtcNow;
            var config = new TCGGameProviderScheduleConfig()
            {
                LastGameRecorDirectory = DateTimeOffset.UtcNow.ToString("yyyyMMdd"),
                LastGameRecorFile = DateTimeOffset.UtcNow.ToString("yyyyMMdd0000_0001.json"),
            };

            var gameProviderSchedule = new GameProviderSchedule()
            {
                Id = GuidExtensions.NewSequentialGuid(),
                GameProviderId = gameProviderId,
                GameProviderCode = GetGameProviderCode(),
                BrandCode = string.Empty,
                AgentId = GetAgentId(subGP, merchantCode),
                CurrencyCode = currencyCode,
                Config = JsonSerializer.Serialize(config),
                CreatedOn = sysTime,
                UpdatedOn = sysTime
            };
            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);

            return gameProviderSchedule;
        }

        public async ValueTask GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, string lastGameRecorDirectory, string lastGameRecorFile)
        {
            var config = GetGameProviderScheduleConfig(gameProviderSchedule);
            if (!string.IsNullOrEmpty(lastGameRecorDirectory) && !string.IsNullOrEmpty(lastGameRecorFile))
            {
                config.LastGameRecorDirectory = lastGameRecorDirectory;
                config.LastGameRecorFile = lastGameRecorFile;
            }
            gameProviderSchedule.Config = JsonSerializer.Serialize(config);
            gameProviderSchedule.UpdatedOn = DateTimeOffset.UtcNow;

            await _gameProviderScheduleRepository.MergeAsync(gameProviderSchedule).ConfigureAwait(false);
        }

        public Task<GetGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId)
            => _gameRepository.GetGameList(gameProviderId, GameType.LotteryLobby);

        public Task<GameProvider> GetGameProviderAsync()
                                            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.TCG);

        public string GetGameProviderCode()
            => GameProviderType.TCG;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.TCG).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode)
         => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, GetAgentId(subGP, merchantCode), currencyCode);

        public ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode, string agentId)
            => _gameProviderScheduleRepository.GetAsync(gameProviderId, string.Empty, agentId, currencyCode);

        public TCGGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule)
            => JsonSerializer.Deserialize<TCGGameProviderScheduleConfig>(gameProviderSchedule.Config);

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public Task<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);

        public (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId)
        {
            var splits = agentId.Split("-");
            return (splits[0], splits[1]);
        }

        private string GetAgentId(string subGP, string merchantCode)
                        => string.Format("{0}-{1}", subGP, merchantCode);
    }
}
