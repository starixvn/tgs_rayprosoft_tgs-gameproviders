using System.Linq;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace Tgs.GameProviders.Services.WMCasino
{
    public class WMCasinoAgentMapper : IWMCasinoAgentMapper
    {
        public WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode)
        {
            if (merchants == null || merchants.Length == 0)
            {
                return null;
            }

            var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                merchants.FirstOrDefault(m => m.BrandCode == CustomConst.BrandCodeDefault);

            return merchant?.Agents.FirstOrDefault(ag => ag.CurrencyCode == currencyCode && ag.Status);
        }
    }
}
