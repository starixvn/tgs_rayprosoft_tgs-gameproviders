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
        private readonly ILogger<WMCasinoService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly ISettingRepository _settingRepository;

        public WMCasinoService(
            ILogger<WMCasinoService> logger,
            ISettingRepository settingRepository,
            IPlayerRepository playerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        }

        public async ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
                .ConfigureAwait(false);

        public async ValueTask<GetPlayer> GetPlayerAsync(string playerCode)
            => await _playerRepository.GetPlayerAsync(playerCode).ConfigureAwait(false);
    }
}