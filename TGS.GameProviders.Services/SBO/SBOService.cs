using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.SBO
{
    internal class SBOService : ISBOService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<SBOService> _logger;
        private readonly ISettingRepository _settingRepository;

        public SBOService(
            ILogger<SBOService> logger,
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async ValueTask<GameModel> GetGameAsync(
            int gameId)
            => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

        public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
            => _gameProviderRepository.GetAsync(gameProviderId);

        public async ValueTask<string> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en_US";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.SBOBET);

        public async ValueTask<SBOConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository.GetConfigurationAsync<SBOConfiguration>(GameProviderConfigurations.SBOBET)
                .ConfigureAwait(false);
    }
}
