using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.KY;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.KY;

public interface IKYService
{
    ValueTask<string> GetAgentIdAsync(string brandCode, string currencyCode);

    ValueTask<KYBrandMerchant> GetBrandMerchantAsync(string brandCode);

    GameRoundClientStatus GetClientRound(decimal playerWinLoss);

    ValueTask<Guid> GetGameProviderIdAsync();

    bool IgnoreCaseEquals(string source, string trigger);
}
