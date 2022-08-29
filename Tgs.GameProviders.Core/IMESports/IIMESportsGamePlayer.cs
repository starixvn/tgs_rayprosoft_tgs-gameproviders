using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration.IMESports;

namespace Tgs.GameProviders.Core.IMESports;

public interface IIMESportsGamePlayer
{
    ValueTask<bool> CreatePlayerAsync(IMESportsBrandMerchant merchant, string imesPlayerId, string gameProviderCurrencyCode);
}
