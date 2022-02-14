using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.Core.KY
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
