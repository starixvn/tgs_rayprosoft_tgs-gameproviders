using Tgs.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace Tgs.GameProviders.Core.WMCasino
{
    public interface IWMCasinoAgentMapper
    {
        WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode);
    }
}
