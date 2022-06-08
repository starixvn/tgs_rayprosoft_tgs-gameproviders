using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Services.Joker;

internal class JokerService : IJokerService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ISettingRepository _settingRepository;

    public JokerService(
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        ISettingRepository settingRepository)
    {
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public ValueTask<GameModel> GetGameAsync(int gameId)
        => _gameRepository.GetAsync(gameId);

    public ValueTask<Guid> GetGameProviderIdAsync()
        => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Joker);

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(string cultureCode)
    {
        var gameProviderId = await GetGameProviderIdAsync().ConfigureAwait(false);
        var gameProviderCultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<JokerConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<JokerConfiguration>(GameProviderConfigurations.Joker)
            .ConfigureAwait(false);
}
