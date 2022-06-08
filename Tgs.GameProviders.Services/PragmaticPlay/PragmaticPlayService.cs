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
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameRepository _gameRepository;
    private readonly ISettingRepository _settingRepository;

    public PragmaticPlayService(
        IGameProviderRepository gameProviderRepository,
        IGameRepository gameRepository,
        ISettingRepository settingRepository)
    {
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<GameModel> GetGameAsync(int gameId)
        => await _gameRepository.GetAsync(gameId).ConfigureAwait(false);

    public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderRepository.GetAsync(gameProviderId);

    public async ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<PragmaticPlayConfiguration>(GameProviderConfigurations.PragmaticPlay)
            .ConfigureAwait(false);
}
