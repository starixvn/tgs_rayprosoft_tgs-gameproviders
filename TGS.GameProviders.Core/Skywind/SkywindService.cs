using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.Skywind
{
    internal class SkywindService : ISkywindService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingRepository;

        public SkywindService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public Task<GameProvider> GetGameProviderAsync()
            => _gameProviderRepository.GetGameProviderAsync(GameProviderType.Skywind);

        public string GetGameProviderCode()
            => GameProviderType.SexyGaming;

        public async Task<Guid> GetGameProviderIdAsync()
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.Skywind).ConfigureAwait(false);
            return gameProvider.Id;
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.Skywind).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProvider.Id, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "en";
        }


        public Task<SkywindConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<SkywindConfiguration>(GameProviderConfigurations.Skywind);
    }
}
