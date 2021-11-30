using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Saba;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.Saba
{
    internal class SabaService : ISabaService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ILogger<SabaService> _logger;
        private readonly ISettingRepository _settingRepository;

        public SabaService(
            ILogger<SabaService> logger,
            IGameProviderRepository gameProviderRepository,
            ISettingRepository settingRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
        }

        public async ValueTask<string> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        {
            var (isCurrencyCode, gameProviderCurrencyCode) = await _gameProviderRepository
                .GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode)
                .ConfigureAwait(false);

            return isCurrencyCode ? gameProviderCurrencyCode : null;
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.Saba);

        public async ValueTask<string> GetGameProviderLanguageCodeAsync(Guid gameProviderId, string cultureCode)
        {
            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode)
                .ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en";
        }

        public async ValueTask<SabaConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
            .GetConfigurationAsync<SabaConfiguration>(GameProviderConfigurations.Saba)
            .ConfigureAwait(false);

        public bool IsTestEnvironment(IEnumerable<string> testEnvironments, string currentEnvironmentName)
            => testEnvironments.Any(env => env.Equals(currentEnvironmentName, StringComparison.OrdinalIgnoreCase));
    }
}