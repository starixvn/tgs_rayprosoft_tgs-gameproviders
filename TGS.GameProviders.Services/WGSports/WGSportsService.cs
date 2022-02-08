using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGSports
{
    internal class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ISettingRepository _settingRepository;

        public WGSportsService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public decimal? GetAmount(string amountString)
            => decimal.TryParse(amountString, out decimal amount) ? amount : null;

        public ClientRoundStatus GetClientRound(string playerWinLoss)
        {
            try
            {
                var winLoss = decimal.Parse(playerWinLoss);

                return winLoss switch
                {
                    > 0 => ClientRoundStatus.Win,
                    <= 0 => ClientRoundStatus.Loss,
                };
            }
            catch
            {
                return ClientRoundStatus.Void;
            }
        }

        public async ValueTask<GameInfo> GetGameByExternalGameId(Guid gameProviderId, string externalGameId)
            => await _gameService.GetAsync(gameProviderId, externalGameId).ConfigureAwait(false);

        public string GetGameProviderCode()
            => GameProviderType.WGSports;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGSports);

        public Task<WGSportsConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<WGSportsConfiguration>(GameProviderConfigurations.WGSports);
    }
}
