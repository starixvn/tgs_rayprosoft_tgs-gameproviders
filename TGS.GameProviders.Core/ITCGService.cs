using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ITCGService
    {
        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        Task<TCGConfiguration> GetSettingConfigurationAsync();

        Task<GetGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId);

        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        ValueTask GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, string lastGameRecorDirectory, string lastGameRecorFile);

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode, string agentId);

        TCGGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId);

    }
}
