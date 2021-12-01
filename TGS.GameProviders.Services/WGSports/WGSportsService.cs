using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.WGSports;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGSports
{
    internal class WGSportsService : IWGSportsService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ISettingRepository _settingRepository;

        public WGSportsService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public ClientRoundStatus GetClientRound(decimal playerWinLoss)
           => playerWinLoss switch
           {
               > 0 => ClientRoundStatus.Win,
               <= 0 => ClientRoundStatus.Loss,
           };

        public async ValueTask<GameModel> GetGameByExternalGameId(Guid gameProviderId, string externalGameId)
            => await _gameRepository.GetAsync(gameProviderId, externalGameId).ConfigureAwait(false);

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
