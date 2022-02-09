using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AllBet
{
    internal class AllBetService : IAllBetService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly ILogger<AllBetService> _logger;
        private readonly ISettingService _settingService;

        public AllBetService(
            ILogger<AllBetService> logger,
            ISettingService settingService,
            IGameProviderService gameProviderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<string> GetGameProviderLanguageAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.AllBet)
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
