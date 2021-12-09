using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaService
    {

        string GetGameProviderCode();

        ValueTask<Guid> GetGameProviderIdAsync();

        Task<AviaConfiguration> GetSettingConfigurationAsync();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);
    }
}
