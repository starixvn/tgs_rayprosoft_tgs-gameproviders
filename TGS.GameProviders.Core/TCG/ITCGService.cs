using System;
using System.Threading.Tasks;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.TCG;

namespace TGS.GameProviders.Core.TCG
{
    public interface ITCGService
    {
        Task<GetGame[]> GetGameLotteryLobbyAsync(Guid gameProviderId);

        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        Task<TCGConfiguration> GetSettingConfigurationAsync();

        (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId);
    }
}
