using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Habanero
{
    internal class HabaneroService : IHabaneroService
    {
        private readonly IBrandService _brandService;
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ILogger<HabaneroService> _logger;
        private readonly IPlayerService _playerService;
        private readonly ISettingService _settingService;

        public HabaneroService(
            ILogger<HabaneroService> logger,
            IBrandService brandService,
            IGameProviderService gameProviderService,
            IGameService gameService,
            IPlayerService playerService,
            ISettingService settingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        public async ValueTask<GameInfo> GetGameAsync(int gameId)
            => await _gameService.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, Core.PlatformType platformType)
        {
            var configurationId = await _brandService
                .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
                .ConfigureAwait(false);

            var (desktopEndpoint, mobileEndpoint) = await _gameProviderService
                .GetGameEndpointsAsync(configurationId)
                .ConfigureAwait(false);

            return platformType == Core.PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
        }

        public ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId)
            => _gameProviderService.GetAsync(gameProviderId);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var gameProviderCultureCode = await _gameProviderService
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }

        public async ValueTask<PlayerInfo> GetPlayerAsync(string playerCode)
            => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<HabaneroConfiguration> GetSettingConfigurationAsync()
            => await _settingService
                .GetConfigurationAsync<HabaneroConfiguration>(GameProviderConfigurations.Habanero)
                .ConfigureAwait(false);
    }
}
