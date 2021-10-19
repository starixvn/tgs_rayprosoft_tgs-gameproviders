using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.TCG;
using TGS.GameProviders.MongoRepositories.Models;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.TCG;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ITCGService
    {
        ValueTask<GameProviderSchedule> GameProviderScheduleInitAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        ValueTask GameProviderScheduleUpdateAsync(GameProviderSchedule gameProviderSchedule, string lastGameRecorDirectory, string lastGameRecorFile);

        Task<GetGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId);

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string subGP, string currencyCode, string merchantCode);

        ValueTask<GameProviderSchedule> GetGameProviderScheduleAsync(Guid gameProviderId, string currencyCode, string agentId);

        TCGGameProviderScheduleConfig GetGameProviderScheduleConfig(GameProviderSchedule gameProviderSchedule);

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        Task<TCGConfiguration> GetSettingConfigurationAsync();

        (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId);
    }
}
