using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Skywind;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Skywind
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

        public string GetGameProviderCode()
            => GameProviderType.SexyGaming;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Skywind).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "en";
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Skywind);

        public Task<SkywindConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<SkywindConfiguration>(GameProviderConfigurations.Skywind);
    }
}
