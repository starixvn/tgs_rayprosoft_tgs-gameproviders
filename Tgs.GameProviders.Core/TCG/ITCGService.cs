using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.TCG;

namespace Tgs.GameProviders.Core.TCG
{
    public interface ITCGService
    {
        IAsyncEnumerable<GameInfo> GetGameLotteryLobbyAsync(Guid gameProviderId);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<TCGMerchant> GetMerchantAsync(string currencyCode);

        ValueTask<TCGMerchant> GetMerchantByMerchantCodeAsync(string merchantCode);

        ValueTask<TCGConfiguration> GetSettingConfigurationAsync();

        (string SubGP, string MerchantCode) GetSubGPAndMerchantCode(string agentId);
    }
}
