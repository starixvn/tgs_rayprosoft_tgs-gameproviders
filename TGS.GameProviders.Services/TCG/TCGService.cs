using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;

namespace TGS.GameProviders.Services.TCG
{
    internal class TCGService : ITCGService
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameService _gameService;
        private readonly ISettingRepository _settingRepository;

        public TCGService(
            ISettingRepository settingRepository,
            IGameProviderRepository gameProviderRepository,
            IGameService gameService
            )
        {
            _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public IAsyncEnumerable<GameInfo> GetGameLotteryLobbyAsync(Guid gameProviderId)
            => _gameService.GetGameListAsync(gameProviderId, Core.GameType.LotteryLobby);

        public string GetGameProviderCode()
            => GameProviderType.TCG;

        public async ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode)
        {
            var gameProviderId = await _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.TCG).ConfigureAwait(false);
            var (IsCultureCode, GameProviderCultureCode) = await _gameProviderRepository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);
            return (IsCultureCode) ? GameProviderCultureCode : "EN";
        }

        public ValueTask<Guid> GetGameProviderIdAsync()
            => _gameProviderRepository.GetGameProviderIdAsync(GameProviderType.TCG);

        public async ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
        }

        public async ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode)
        {
            var configuration = await _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG).ConfigureAwait(false);
            return configuration.Merchants.Where(x => x.MerchantCode == merchantCode).FirstOrDefault();
        }

        public Task<TCGConfiguration> GetSettingConfigurationAsync()
            => _settingRepository.GetConfigurationAsync<TCGConfiguration>(GameProviderConfigurations.TCG);

        public (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId)
        {
            var splits = agentId.Split("-");
            return (splits[0], splits[1]);
        }

        private string GetAgentId(string subGP, string merchantCode)
                        => string.Format("{0}-{1}", subGP, merchantCode);
    }
}
