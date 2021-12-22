using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaGamePlayerService
    {
        ValueTask<bool> CreatePlayerAsync(AviaBrandMerchant brandMerchant, string playerCode, string currencyCode);
    }
}
