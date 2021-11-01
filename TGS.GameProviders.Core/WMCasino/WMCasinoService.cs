using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core.WMCasino
{
    public class WMCasinoService : IWMCasinoService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ILogger<WMCasinoService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public WMCasinoService(
            ILogger<WMCasinoService> logger,
            ISettingRepository settingRepository,
            IPlayerRepository playerRepository,
            IGameProviderRepository gameProviderRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
                .ConfigureAwait(false);

        public async ValueTask<GetPlayer> GetPlayerAsync(string playerCode)
            => await _playerRepository.GetPlayerAsync(playerCode).ConfigureAwait(false);

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProvider = await _gameProviderRepository.GetGameProviderAsync(GameProviderType.WMCasino)
                .ConfigureAwait(false);

            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProvider.Id, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en_US";
        }
    }
}