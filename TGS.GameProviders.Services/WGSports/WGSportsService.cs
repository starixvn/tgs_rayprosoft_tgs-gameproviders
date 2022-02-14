using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGSports
{
    internal class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;
        private readonly ISettingService _settingService;

        public WGSportsService(
            ISettingService settingService,
            IGameProviderService gameProviderService,
            IGameService gameService
            )
        {
            _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public decimal? GetAmount(string amountString)
            => decimal.TryParse(amountString, out decimal amount) ? amount : null;

        public RoundClientStatus GetClientRound(string playerWinLoss)
        {
            try
            {
                var winLoss = decimal.Parse(playerWinLoss);

                return winLoss switch
                {
                    > 0 => RoundClientStatus.Win,
                    <= 0 => RoundClientStatus.Loss,
                };
            }
            catch
            {
                return RoundClientStatus.Void;
            }
        }

        public async ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId)
            => await _gameService.GetAsync(gameProviderId, externalGameId).ConfigureAwait(false);

        public string GetGameProviderCode()
            => GameProviderType.WGSports;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCultureCode = await _gameProviderService.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var gameProviderCurrencyCode = await _gameProviderService.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return !string.IsNullOrEmpty(gameProviderCurrencyCode) ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.WGSports);

        public ValueTask<WGSportsConfiguration> GetSettingConfigurationAsync()
            => _settingService.GetConfigurationAsync<WGSportsConfiguration>(GameProviderConfigurations.WGSports);
    }
}
