using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGHorse
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

        public ClientRoundStatus GetClientRound(decimal playerWinLoss)
            => playerWinLoss switch
            {
                > 0 => ClientRoundStatus.Win,
                0 => ClientRoundStatus.Draw,
                < 0 => ClientRoundStatus.Loss,
            };

        public async ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId)
            => await _gameService.GetAsync(gameProviderId, externalGameId).ConfigureAwait(false);

        public string GetGameProviderCode()
            => GameProviderType.WGHorse;

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
