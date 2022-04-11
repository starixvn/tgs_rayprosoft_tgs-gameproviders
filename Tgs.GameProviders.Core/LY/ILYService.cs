using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.LY;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.LY
{
    public interface ILYService
    {
        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<LYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        RoundClientStatus GetClientRound(decimal playerWinLoss);

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<LYConfiguration> GetSettingConfigurationAsync();
    }
}
