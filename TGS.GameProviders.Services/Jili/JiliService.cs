using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Jili;

internal class JiliService : IJiliService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ILogger<JiliService> _logger;
    private readonly ISettingRepository _settingRepository;

    public JiliService(
        ILogger<JiliService> logger,
        IGameProviderRepository gameProviderRepository,
        ISettingRepository settingRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
    }

    public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
    {
        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
            .ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "en-US";
    }

    public async ValueTask<JiliConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<JiliConfiguration>(GameProviderConfigurations.Jili)
            .ConfigureAwait(false);
}