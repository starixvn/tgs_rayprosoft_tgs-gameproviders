using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.PragmaticPlay;

internal class PragmaticPlayService : IPragmaticPlayService
{
    private readonly IGameProviderService _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ILogger<PragmaticPlayService> _logger;
    private readonly ISettingService _settingService;

    public PragmaticPlayService(
        ILogger<PragmaticPlayService> logger,
        IGameProviderService gameProviderService,
        IGameService gameService,
        ISettingService settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameService.GetAsync(gameId).ConfigureAwait(false);

    public ValueTask<GameProviderInfo> GetGameProviderAsync(Guid gameProviderId)
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
