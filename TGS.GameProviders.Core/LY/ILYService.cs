using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.LY;

namespace TGS.GameProviders.Core.LY
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
