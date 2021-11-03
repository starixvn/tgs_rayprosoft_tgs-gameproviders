using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(SexyGamingConfiguration configuration, string playerCode, string currencyCode, string gameProviderCurrency, string gameProviderCulture);
    }
}
