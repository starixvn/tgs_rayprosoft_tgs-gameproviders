using System.Threading.Tasks;
using TGS.GameProviders.Core.BigGaming.Models.Commons;
using TGS.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace TGS.GameProviders.Core.BigGaming
{
    public interface IBigGamingGameUrl
    {
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GameInfo game, string cultureCode);
    }
}
