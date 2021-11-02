using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core.SBO
{
    public class SBOService : ISBOService
    {
        private static readonly IReadOnlyDictionary<GameType, string> _Portfolios = new Dictionary<GameType, string>
        {
            [GameType.Sportsbook] = "SportsBook",
            [GameType.VirtualSports] = "VirtualSports",
            [GameType.VirtualSportsLobby] = "VirtualSports"
        };

        private readonly IBrandRepository _brandRepository;

        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<SBOService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public SBOService(
            ILogger<SBOService> logger,
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository,
            IBrandRepository brandRepository,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<BrandGameProvider> GetBrandGameProviderAsync(Guid brandId, Guid gameProviderId)
            => await _brandRepository.GetBrandGameProviderAsync(gameProviderId, brandId).ConfigureAwait(false);

        public async ValueTask<GetGame> GetGameAsync(int gameId)
            => await _gameRepository.GetGameAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.SBOBET)
                .ConfigureAwait(false);

            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProvider.Id, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en_US";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.SBOBET)
                .ConfigureAwait(false);

            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(gameProvider.Id, currencyCode).ConfigureAwait(false);
            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public async ValueTask<GetPlayer> GetPlayerAsync(string playerCode)
            => await _playerRepository.GetPlayerAsync(playerCode).ConfigureAwait(false);

        public string GetPortfolio(GameType gameType)
                    => !_Portfolios.TryGetValue(gameType, out var portfolio)
                ? null
                : portfolio;

        public async ValueTask<SBOConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository.GetConfigurationAsync<SBOConfiguration>(GameProviderConfigurations.SBOBET)
                .ConfigureAwait(false);
    }
}
