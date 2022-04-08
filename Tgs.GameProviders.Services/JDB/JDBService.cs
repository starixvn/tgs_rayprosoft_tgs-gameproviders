using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.JDB;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.JDB;

internal class JDBService : IJDBService
{
    private readonly IGameProviderService _gameProviderService;
    private readonly ILogger<JDBService> _logger;
    private readonly ISettingService _settingService;

    public JDBService(
        ILogger<JDBService> logger,
        ISettingService settingService,
        IGameProviderService gameProviderService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
    }

    public async ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en";
    }

    public async ValueTask<JDBConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<JDBConfiguration>(GameProviderConfigurations.JDB)
            .ConfigureAwait(false);
}
