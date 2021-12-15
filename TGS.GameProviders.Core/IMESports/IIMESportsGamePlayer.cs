using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration.IMESports;

namespace TGS.GameProviders.Core.IMESports
{
    public interface IIMESportsGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(IMESportsBrandMerchant merchant, string imesPlayerId, string gameProviderCurrencyCode);
    }
}
