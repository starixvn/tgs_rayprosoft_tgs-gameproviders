using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.PragmaticPlay
{
    internal class PragmaticPlayService : IPragmaticPlayService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<PragmaticPlayService> _logger;
        private readonly ISettingRepository _settingRepository;

        public PragmaticPlayService(
            ILogger<PragmaticPlayService> logger,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            ISettingRepository settingRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameProviderRepository = gameProviderRepository;
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        }

        public async ValueTask<GameModel> GetGameAsync(int gameId)
            => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCurrencyLanguageAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.
                    PragmaticPlay)
                .ConfigureAwait(false);

            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en";
        }

        public async ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<PragmaticPlayConfiguration>(GameProviderConfigurations.PragmaticPlay)
                .ConfigureAwait(false);
    }
}
