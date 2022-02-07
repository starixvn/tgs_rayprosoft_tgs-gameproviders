using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.JDB;

internal class JDBService : IJDBService
{
    private readonly IGameProviderRepository _gameProviderRepository;
    private readonly ILogger<JDBService> _logger;
    private readonly ISettingRepository _settingRepository;

    public JDBService(
        ILogger<JDBService> logger,
        ISettingRepository settingRepository,
        IGameProviderRepository gameProviderRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
    }

    public async ValueTask<string> GetGameProviderLanguageAsync(Guid gameProviderId, string cultureCode)
    {
        var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
            .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        return isCultureCode ? gameProviderCultureCode : "en";
    }

    public async ValueTask<JDBConfiguration> GetSettingConfigurationAsync()
        => await _settingRepository
            .GetConfigurationAsync<JDBConfiguration>(GameProviderConfigurations.JDB)
            .ConfigureAwait(false);
}
