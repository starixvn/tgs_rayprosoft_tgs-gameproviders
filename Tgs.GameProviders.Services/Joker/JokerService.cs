using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerService : IJokerService
{
    private readonly IGameProviderService _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ISettingService _settingService;

    public JokerService(
        IGameProviderService gameProviderService,
        IGameService gameService,
        ISettingService settingService)
    {
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<JokerConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<JokerConfiguration>(GameProviderConfigurations.Joker)
            .ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode)
    {
        var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public ValueTask<GameInfo> GetGameAsync(int gameId)
        => _gameService.GetAsync(gameId);

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderService.GetCacheGameProviderIdAsync(GameProviderType.Joker);
}