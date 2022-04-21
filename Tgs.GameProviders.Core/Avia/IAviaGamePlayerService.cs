using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.Avia;

namespace Tgs.GameProviders.Core.Avia
{
    public interface IAviaGamePlayerService
    {
        ValueTask<bool> CreatePlayerAsync(AviaBrandMerchant brandMerchant, string playerCode, string currencyCode);
    }
}
