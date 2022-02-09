using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.IMESports;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.IMESports
{
    internal class IMESportsService : IIMESportsService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ILogger<IMESportsService> _logger;
        private readonly IPlayerService _playerService;
        private readonly ISettingService _settingService;

        public IMESportsService(
            ILogger<IMESportsService> logger,
            ISettingService settingService,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService,
            IPlayerService playerService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask<GameInfo> GetGameAsync(int gameId)
            => await _gameService.GetAsync(gameId).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.IMESports);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
                .ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "EN";
        }

        public async ValueTask<PlayerInfo> GetPlayerAsync(string playerCode)
            => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<IMESportsConfiguration> GetSettingConfigurationAsync()
            => await _settingService
                .GetConfigurationAsync<IMESportsConfiguration>(GameProviderConfigurations.IMESports)
                .ConfigureAwait(false);
    }
}
