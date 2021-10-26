using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.BigGaming.Models.Commons;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Core
{
    public interface IBigGamingGameUrl
    {
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GetGame game, string cultureCode);
    }
}
