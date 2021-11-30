using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.IMESports
{
    internal class IMESportsService : IIMESportsService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<IMESportsService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public IMESportsService(
            ILogger<IMESportsService> logger,
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public ValueTask<Guid> GetBrandIdAsync(string brandCode)
            => _brandRepository.GetBrandIdAsync(brandCode);

        public async ValueTask<GameModel> GetGameAsync(int gameId)
            => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.IMESports);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "EN";
        }

        public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
            => await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync()
                    => await _settingRepository
                .GetConfigurationAsync<IMESportsConfiguration>(GameProviderConfigurations.IMESports)
                .ConfigureAwait(false);
    }
}