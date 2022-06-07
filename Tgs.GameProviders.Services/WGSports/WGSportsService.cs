using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.WGSports;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Services.WGSports
{
    internal class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderRepository _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ISettingRepository _settingService;

        public WGSportsService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderService,
            IGameService gameService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public decimal GetAmount(string amountString)
            => decimal.TryParse(amountString, out decimal amount) ? amount : 0;

        public GameRoundClientStatus GetClientRound(string playerWinLoss)
        {
            try
            {
                var winLoss = decimal.Parse(playerWinLoss);

                return winLoss switch
                {
                    > 0 => GameRoundClientStatus.Win,
                    <= 0 => GameRoundClientStatus.Loss,
                };
            }
            catch
            {
                return GameRoundClientStatus.Void;
            }
        }

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetGameProviderIdAsync(GameProviderType.WGSports);

        public ValueTask<WGSportsConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<WGSportsConfiguration>(GameProviderConfigurations.WGSports);
    }
}
