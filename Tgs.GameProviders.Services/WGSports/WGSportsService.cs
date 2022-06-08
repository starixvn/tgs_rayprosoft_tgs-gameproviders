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
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ISettingRepository _settingService;

        public WGSportsService(
            ISettingRepository settingService,
            IGameProviderRepository gameProviderRepository)
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
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
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports);

        public ValueTask<WGSportsConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<WGSportsConfiguration>(GameProviderConfigurations.WGSports);
    }
}
