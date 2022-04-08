using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.KY;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.KY
{
    public interface IKYService
    {
        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<KYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        RoundClientStatus GetClientRound(decimal playerWinLoss);

        DateTimeOffset GetDateTimeOffsetTimeZone8();

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<KYConfiguration> GetSettingConfigurationAsync();
    }
}
