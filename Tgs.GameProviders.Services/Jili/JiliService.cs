using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Jili;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Services.Jili;

internal class JiliService : IJiliService
{
    private readonly IGameProviderRepository _gameProviderService;
    private readonly ILogger<JiliService> _logger;
    private readonly ISettingRepository _settingService;

    public JiliService(
        ILogger<JiliService> logger,
        IGameProviderRepository gameProviderService,
        ISettingRepository settingService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
    }

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var gameProviderCultureCode = await _gameProviderService
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return !string.IsNullOrEmpty(gameProviderCultureCode) ? gameProviderCultureCode : "en-US";
    }

    public async ValueTask<JiliConfiguration> GetSettingConfigurationAsync()
        => await _settingService
            .GetConfigurationAsync<JiliConfiguration>(GameProviderConfigurations.Jili)
            .ConfigureAwait(false);
}
