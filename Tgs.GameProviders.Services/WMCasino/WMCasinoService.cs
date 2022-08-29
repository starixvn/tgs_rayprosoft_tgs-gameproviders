using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.WMCasino;

public class WMCasinoService : IWMCasinoService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly ILogger<WMCasinoService> _logger;
    private readonly IPlayerRepository _playerService;
    private readonly ISettingRepository _settingService;

    public WMCasinoService(
        ILogger<WMCasinoService> logger,
        ISettingRepository settingService,
        IPlayerRepository playerService,
        IGameProviderRepository gameProviderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
    }

    public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
    {
        var gameProviderId = await _gameProviderService.GetGameProviderIdAsync(GameProviderType.WMCasino)
            .ConfigureAwait(false);

        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en_US";
    }

    public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
        => await _playerService.GetAsync(playerCode).ConfigureAwait(false);

    public async ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
            .ConfigureAwait(false);
}
