using Tgs.GameProviders.Domains.SettingConfiguration.AllBet;

namespace Tgs.GameProviders.Core.AllBet;

public interface IAllBetAgentMapper
{
    AllBetAgent GetAgent(AllBetBrandMerchant[] merchants, string brandCode, string currencyCode);
}
