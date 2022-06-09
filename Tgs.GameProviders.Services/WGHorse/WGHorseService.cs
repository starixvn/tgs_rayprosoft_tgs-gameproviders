using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.WGHorse;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.WGHorse
{
    internal class WGHorseService : IWGHorseService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ISettingService _settingService;

        public WGHorseService(
            ISettingService settingService,
            IGameProviderService gameProviderService,
            IGameService gameService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public GameRoundClientStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => GameRoundClientStatus.Win,
                0 => GameRoundClientStatus.Draw,
                < 0 => GameRoundClientStatus.Loss,
            };

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGHorse);

        public ValueTask<WGHorseConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<WGHorseConfiguration>(GameProviderConfigurations.WGHorse);
    }
}
