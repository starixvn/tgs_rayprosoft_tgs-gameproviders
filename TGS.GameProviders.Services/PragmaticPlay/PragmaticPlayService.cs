using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.PragmaticPlay;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.GameProvider;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.PragmaticPlay;

internal class PragmaticPlayService : IPragmaticPlayService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly IGameService _gameService;
    private readonly ILogger<PragmaticPlayService> _logger;
    private readonly ISettingRepository _settingRepository;

    public PragmaticPlayService(
        ILogger<PragmaticPlayService> logger,
        IGameProviderRepository gameProviderRepository,
        IGameService gameService,
        ISettingRepository settingRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderRepository = gameProviderRepository;
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<GameInfo> GetGameAsync(int gameId)
        => await _gameService.GetAsync(gameId).ConfigureAwait(false);

    public ValueTask<GameProviderModel> GetGameProviderAsync(Guid gameProviderId)
        => _gameProviderRepository.GetAsync(gameProviderId);

    public async ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode)
    {
        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "en";
    }

    public async ValueTask<PragmaticPlayConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<PragmaticPlayConfiguration>(GameProviderConfigurations.PragmaticPlay)
            .ConfigureAwait(false);
}
