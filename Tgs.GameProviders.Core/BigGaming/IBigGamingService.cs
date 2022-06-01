using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.BigGaming;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameRounds.Common;

namespace Tgs.GameProviders.Core.BigGaming
{
    public interface IBigGamingService
    {
        ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode);

        GameRoundClientStatus GetClientRound(decimal winLoss);

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<Guid> GetGameProviderIdAsync();
    }
}
