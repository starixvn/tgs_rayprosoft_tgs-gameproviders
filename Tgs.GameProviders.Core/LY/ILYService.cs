using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.LY;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.LY;

public interface ILYService
{
    ValueTask<string> GetAgentIdAsync(string brandCode, string currencyCode);

    ValueTask<LYBrandMerchant> GetBrandMerchantAsync(string brandCode);

    GameRoundClientStatus GetClientRound(decimal playerWinLoss);

    ValueTask<Guid> GetGameProviderIdAsync();

    bool IgnoreCaseEquals(string source, string trigger);
}
