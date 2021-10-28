using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface ISexyGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(SexyGamingConfiguration configuration, string playerCode, string currencyCode, string gameProviderCurrency, string gameProviderCulture);
    }
}
