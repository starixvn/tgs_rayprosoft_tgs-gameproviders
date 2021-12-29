using TGS.GameProviders.Domains.SettingConfiguration.AllBet;

namespace TGS.GameProviders.Core.AllBet;

public interface IAllBetAgentMapper
{
    AllBetAgent GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode);
}
