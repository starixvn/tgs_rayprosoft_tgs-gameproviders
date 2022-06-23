using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.AllBet
{
    internal class AllBetService : IAllBetService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly ILogger<AllBetService> _logger;
        private readonly ISettingRepository _settingService;

        public AllBetService(
            ILogger<AllBetService> logger,
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<string> GetGameProviderLanguageAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.AllBet)
                .ConfigureAwait(false);

            var gameProviderCultureCode = await _gameProviderService
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
        }

        public async ValueTask<AllBetConfiguration> GetSettingConfigurationAsync()
            => await _settingService
                .GetConfigurationAsync<AllBetConfiguration>(GameProviderConfigurations.AllBet)
                .ConfigureAwait(false);

        public string ToAllBetPlayerId(string playerCode, string suffix)
            => $"{playerCode}_{suffix}";
    }
}
