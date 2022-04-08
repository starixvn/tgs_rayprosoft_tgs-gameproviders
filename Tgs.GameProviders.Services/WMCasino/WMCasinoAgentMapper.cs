using System.Linq;
using Tgs.GameProviders.Core.WMCasino;
using Tgs.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace Tgs.GameProviders.Services.WMCasino
{
    public class WMCasinoAgentMapper : IWMCasinoAgentMapper
    {
        private static readonly string _DefaultCode = "Default";

        public WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode)
        {
            if (merchants == null || merchants.Length == 0)
            {
                return null;
            }

            var merchant = merchants.FirstOrDefault(m => m.BrandCode == brandCode) ??
                merchants.FirstOrDefault(m => m.BrandCode == _DefaultCode);

            return merchant?.Agents.FirstOrDefault(ag => ag.CurrencyCode == currencyCode && ag.Status);
        }
    }
}
