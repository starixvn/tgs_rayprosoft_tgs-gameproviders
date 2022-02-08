using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Evolution;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Evolution
{
    internal class EvolutionService : IEvolutionService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingService _settingService;

        public EvolutionService(
            ISettingService settingService,
            IGameProviderRepository gameProviderRepository
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public string GetGameProviderCode()
            => GameProviderType.Evolution;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Evolution).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "en";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Evolution).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public async Task<Guid> GetGameProviderIdAsync()
            => await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Evolution).ConfigureAwait(false);

        public ValueTask<EvolutionConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<EvolutionConfiguration>(GameProviderConfigurations.Evolution);
    }
}
