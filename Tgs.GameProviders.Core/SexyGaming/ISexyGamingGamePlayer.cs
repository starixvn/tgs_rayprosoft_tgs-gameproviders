using System.Threading.Tasks;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.SexyGaming
{
    public interface ISexyGamingGamePlayer
    {
        ValueTask<bool> CreatePlayerAsync(SexyGamingConfiguration configuration, string playerCode, string currencyCode, string gameProviderCurrency, string gameProviderCulture);
    }
}
