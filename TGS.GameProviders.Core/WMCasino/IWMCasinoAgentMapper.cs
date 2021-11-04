using TGS.GameProviders.Domains.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoAgentMapper
    {
        WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode);
    }
}
