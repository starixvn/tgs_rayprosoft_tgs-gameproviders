using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.KY;

namespace TGS.GameProviders.Core.KY
{
    public interface IKYService
    {
        Task<string> GetAgentIdAsync(string brandCode, string currencyCode);

        Task<KYBrandMerchant> GetBrandMerchantAsync(string brandCode);

        ClientRoundStatus GetClientRound(decimal playerWinLoss);

        DateTimeOffset GetDateTimeOffsetTimeZone8();

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        Task<KYConfiguration> GetSettingConfigurationAsync();
    }
}
