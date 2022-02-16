using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Jili;

internal class JiliService : IJiliService
{
    private readonly IGameProviderService _gameProviderService;
    private readonly ILogger<JiliService> _logger;
    private readonly ISettingService _settingService;

    public JiliService(
        ILogger<JiliService> logger,
        IGameProviderService gameProviderService,
        ISettingService settingService)
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
