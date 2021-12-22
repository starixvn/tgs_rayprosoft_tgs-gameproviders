using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.AllBet;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Services.AllBet
{
    internal class AllBetService : IAllBetService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly ILogger<AllBetService> _logger;
        private readonly ISettingRepository _settingRepository;

        public AllBetService(
            ILogger<AllBetService> logger,
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async ValueTask<string> GetGameProviderLanguageAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.AllBet)
                .ConfigureAwait(false);

            var (isCultureCode, gameProviderCultureCode) = await _gameProviderRepository
                .GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

            return isCultureCode ? gameProviderCultureCode : "en";
        }

        public async ValueTask<AllBetConfiguration> GetSettingConfigurationAsync()
            => await _settingRepository
                .GetConfigurationAsync<AllBetConfiguration>(GameProviderConfigurations.AllBet)
                .ConfigureAwait(false);

        public string ToAllBetPlayerId(string playerCode, string suffix)
            => $"{playerCode}_{suffix}";
    }
}
