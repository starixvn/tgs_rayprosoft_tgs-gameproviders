using System.Threading.Tasks;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;
using Tgs.GameProviders.Domains.SettingConfiguration.BigGaming;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.BigGaming;

public interface IBigGamingGameUrl
{
    ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GameModel game, string cultureCode);
}
