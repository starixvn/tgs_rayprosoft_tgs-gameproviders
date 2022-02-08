using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;

using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WMCasino;

public class WMCasinoService : IWMCasinoService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<WMCasinoService> _logger;
    private readonly IPlayerService _playerService;
    private readonly ISettingRepository _settingRepository;

    public WMCasinoService(
        ILogger<WMCasinoService> logger,
        ISettingRepository settingRepository,
        IPlayerService playerService,
        IGameProviderRepository gameProviderRepository,
        IGameService gameService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _gameProviderRepository =
            gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
    }

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameService.GetAsync(gameId).ConfigureAwait(false);

    public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderRepository.GetAsync(gameProviderId);

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
        => await _settingRepository
            .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
            .ConfigureAwait(false);
}
