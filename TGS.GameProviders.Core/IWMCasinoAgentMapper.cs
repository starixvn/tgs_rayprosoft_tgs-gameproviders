using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.WMCasino;

namespace TGS.GameProviders.Core
{
    public interface IWMCasinoAgentMapper
    {
        WMCasinoAgent GetAgent(WMCasinoBrandMerchant[] merchants, string brandCode, string currencyCode);
    }
}