using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.SexyGaming
{
    internal class SexyGamingService : ISexyGamingService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public SexyGamingService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.SexyGaming);

        public string GetGameProviderCode()
            => GameProviderType.SexyGaming;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.SexyGaming).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public Task<SexyGamingConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<SexyGamingConfiguration>(GameProviderConfigurations.SexyGaming);
    }
}
