using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Habanero
{
    internal class HabaneroService : IHabaneroService
    {
        private readonly IBrandService _brandService;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ILogger<HabaneroService> _logger;
        private readonly IPlayerService _playerService;
        private readonly ISettingRepository _settingRepository;

        public HabaneroService(
            ILogger<HabaneroService> logger,
            IBrandService brandService,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IPlayerService playerService,
            ISettingRepository settingRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        }

        public async ValueTask<GameInfo> GetGameAsync(int gameId)
            => await _gameService.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, Core.PlatformType platformType)
        {
            var configurationId = await _brandService
                .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
                .ConfigureAwait(false);

            var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
                .GetGameEndpointsAsync(configurationId)
                .ConfigureAwait(false);

            return platformType == Core.PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
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

        public async ValueTask<PlayerInfo> GetPlayerAsync(string playerCode)
            => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<HabaneroConfiguration>(GameProviderConfigurations.Habanero)
                .ConfigureAwait(false);
    }
}
