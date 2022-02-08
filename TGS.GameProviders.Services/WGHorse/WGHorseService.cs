using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.WGHorse;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WGHorse
{
    internal class WGHorseService : IWGHorseService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ISettingRepository _settingRepository;

        public WGHorseService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
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
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "2";
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGHorse).ConfigureAwait(false);
            var (IsCurrencyCode, GameProviderCurrencyCode) = await _gameProviderRepository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);
            return (IsCurrencyCode) ? GameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WGHorse);

        public Task<WGHorseConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<WGHorseConfiguration>(GameProviderConfigurations.WGHorse);
    }
}
