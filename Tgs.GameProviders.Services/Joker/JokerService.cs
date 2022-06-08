using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerService : IJokerService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly IGameService _gameService;
    private readonly ISettingRepository _settingService;

    public JokerService(
        IGameProviderRepository gameProviderService,
        IGameService gameService,
        ISettingRepository settingService)
    {
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public ValueTask<GameInfo> GetGameAsync(int gameId)
        => _gameService.GetAsync(gameId);

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderService.GetGameProviderIdAsync(GameProviderType.Joker);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode)
    {
        var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<JokerConfiguration> GetSettingConfigurationAsync()
                    => await _settingService
            .GetConfigurationAsync<JokerConfiguration>(GameProviderConfigurations.Joker)
            .ConfigureAwait(false);
}
