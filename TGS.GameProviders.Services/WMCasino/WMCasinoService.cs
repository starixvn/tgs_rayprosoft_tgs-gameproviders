using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Core.WMCasino;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.WMCasino;

public class WMCasinoService : IWMCasinoService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<WMCasinoService> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly ISettingRepository _settingRepository;

    public WMCasinoService(
        ILogger<WMCasinoService> logger,
        ISettingRepository settingRepository,
        IPlayerRepository playerRepository,
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _gameProviderRepository =
            gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
    }

    public async ValueTask<GameModel> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

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

    public async ValueTask<PlayerModel> GetPlayerAsync(string playerCode)
        => await _playerRepository.GetAsync(playerCode).ConfigureAwait(false);

    public async ValueTask<WMCasinoConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<WMCasinoConfiguration>(GameProviderConfigurations.WMCasino)
            .ConfigureAwait(false);
}
