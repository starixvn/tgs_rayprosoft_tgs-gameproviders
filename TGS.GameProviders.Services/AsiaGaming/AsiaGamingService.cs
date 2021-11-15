using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.AsiaGaming;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AsiaGaming
{
    internal class AsiaGamingService : IAsiaGamingService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<AsiaGamingService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public AsiaGamingService(
            ILogger<AsiaGamingService> logger,
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IPlayerRepository playerRepository,
            IGameRepository gameRepository,
            IBrandRepository brandRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async ValueTask<Guid> GetBrandIdAsync(string brandCode)
            => await _brandRepository.GetBrandIdAsync(brandCode).ConfigureAwait(false);

        public async ValueTask<GetGame> GetGameAsync(int gameId)
            => await _gameRepository.GetGameAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameDomainAsync(Guid brandId, Guid gameProviderId, PlatformType platformType)
        {
            var gameProviderConfigurationId = await _brandRepository
                .GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId).ConfigureAwait(false);

            var (desktopEndpoint, mobileEndpoint) = await _gameProviderRepository
                .GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

            return platformType == PlatformType.Mobile ? mobileEndpoint : desktopEndpoint;
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public async ValueTask<string> GetGameProviderCurrencyLanguageAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en_US";
        }

        public async ValueTask<Guid> GetGameProviderIdAsync()
            => await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.AsiaGaming)
                .ConfigureAwait(false);

        public async ValueTask<decimal?> GetPlayerBalanceAsync(string playerCode)
            => await _playerRepository.GetPlayerBalanceAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<AsiaGamingConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<AsiaGamingConfiguration>(GameProviderConfigurations.AsiaGaming)
                .ConfigureAwait(false);
    }
}
