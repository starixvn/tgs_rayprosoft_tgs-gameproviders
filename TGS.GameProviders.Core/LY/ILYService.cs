using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.LY;

namespace TGS.GameProviders.Core.LY
{
    public interface ILYService
    {
        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<LYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<LYConfiguration> GetSettingConfigurationAsync();
    }
}
