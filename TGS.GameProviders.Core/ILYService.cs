using System;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.LY;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Core
{
    public interface ILYService
    {
        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<LYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        DateTimeOffset GetDateTimeOffsetTimeZone8();

        Task<GameProvider> GetGameProviderAsync();

        string GetGameProviderCode();

        Task<Guid> GetGameProviderIdAsync();

        Task<LYConfiguration> GetSettingConfigurationAsync();
    }
}
