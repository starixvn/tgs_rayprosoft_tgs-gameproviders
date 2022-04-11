using System.Threading.Tasks;
using Tgs.GameProviders.Core.BigGaming.Models.Commons;
using Tgs.GameProviders.Domains.SettingConfiguration.BigGaming;

namespace Tgs.GameProviders.Core.BigGaming
{
    public interface IBigGamingGameUrl
    {
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, string playerCode, GameInfo game, string cultureCode);
    }
}
