using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.PragmaticPlay;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.PragmaticPlay;

internal class PragmaticPlayService : IPragmaticPlayService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ILogger<PragmaticPlayService> _logger;
    private readonly ISettingRepository _settingService;

    public PragmaticPlayService(
        ILogger<PragmaticPlayService> logger,
        IGameProviderRepository gameProviderService,
        IGameService gameService,
        ISettingRepository settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameService.GetAsync(gameId).ConfigureAwait(false);

    public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderService.GetAsync(gameProviderId);

    public async ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<PragmaticPlayConfiguration>(GameProviderConfigurations.PragmaticPlay)
            .ConfigureAwait(false);
}
