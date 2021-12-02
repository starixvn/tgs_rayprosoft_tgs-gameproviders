using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Habanero
{
    internal class HabaneroService : IHabaneroService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<HabaneroService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public HabaneroService(
            ILogger<HabaneroService> logger,
            IBrandRepository brandRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            ISettingRepository settingRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        }

        public async ValueTask<GameModel> GetGameAsync(int gameId)
            => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType)
        {
            var configurationId = await _brandRepository
                .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
                .ConfigureAwait(false);

            var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
                .GetGameEndpointsAsync(configurationId)
                .ConfigureAwait(false);

            return platformType == PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
        }

        public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
            => _gameProviderRepository.GetAsync(gameProviderId);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
                .ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en";
        }

        public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
            => await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<HabaneroConfiguration>(GameProviderConfigurations.Habanero)
                .ConfigureAwait(false);
    }
}