using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WMCasino;

public class WMCasinoService : IWMCasinoService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ILogger<WMCasinoService> _logger;
    private readonly IPlayerService _playerService;
    private readonly ISettingService _settingService;

    public WMCasinoService(
        ILogger<WMCasinoService> logger,
        ISettingService settingService,
        IPlayerService playerService,
        IGameProviderRepository gameProviderRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameProviderRepository =
            gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
    }

    public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
    {
        var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.WMCasino)
            .ConfigureAwait(false);

        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<PlayerInfo> GetPlayerAsync(string playerCode)
        => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

    public async ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
            .ConfigureAwait(false);
}
