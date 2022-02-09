using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.SBO;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.SBO
{
    internal class SBOService : ISBOService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ILogger<SBOService> _logger;
        private readonly ISettingService _settingService;

        public SBOService(
            ILogger<SBOService> logger,
            ISettingService settingService,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public async ValueTask<GameInfo> GetGameAsync(
            int gameId)
            => await _gameService.GetAsync(gameId).ConfigureAwait(false);

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
            => await _settingService.GetConfigurationAsync<SBOConfiguration>(GameProviderConfigurations.SBOBET)
                .ConfigureAwait(false);
    }
}
