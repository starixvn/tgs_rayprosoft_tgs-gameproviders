using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Domains.Models.SettingConfiguration;

namespace TGS.GameProviders.Core
{
    public interface ISexyGamingGamePlayerCreate
    {
        ValueTask<bool> CreatePlayerAsync(SexyGamingConfiguration configuration, string playerCode, string currencyCode, string gameProviderCurrency, string gameProviderCulture);
    }
}
