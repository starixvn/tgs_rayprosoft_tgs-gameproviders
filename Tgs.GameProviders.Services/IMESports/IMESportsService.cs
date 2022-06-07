using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.IMESports;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.IMESports
{
    internal class IMESportsService : IIMESportsService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ILogger<IMESportsService> _logger;
        private readonly IPlayerRepository _playerService;
        private readonly ISettingRepository _settingService;

        public IMESportsService(
            ILogger<IMESportsService> logger,
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService,
            IGameService gameService,
            IPlayerRepository playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<GameInfo> GetGameAsync(int gameId)
            => await _gameService.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var gameProviderCurrencyCode = await _gameProviderService
                .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetGameProviderIdAsync(GameProviderType.IMESports);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var gameProviderCultureCode = await _gameProviderService
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
                .ConfigureAwait(false);

            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "EN";
        }

        public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
            => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync()
            => await _settingService
                .GetConfigurationAsync<IMESportsConfiguration>(GameProviderConfigurations.IMESports)
                .ConfigureAwait(false);
    }
}
