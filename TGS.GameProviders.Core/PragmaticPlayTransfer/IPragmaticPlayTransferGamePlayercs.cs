using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer;

public interface IPragmaticPlayTransferGamePlayer
{
    ValueTask<bool> CreatePlayerAsync(
        string playerCode,
        string gameProviderCurrencyCode,
        PragmaticPlayTransferConfiguration configuration);
}
