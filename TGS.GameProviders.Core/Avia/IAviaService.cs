using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.Avia
{
    public interface IAviaService
    {
        string GetGameProviderCode();

        ValueTask<string> GetGameProviderCultureCodeAsync(string cultureCode);

        ValueTask<string> GetGameProviderCurrencyCodeAsync(string currencyCode);

        ValueTask<Guid> GetGameProviderIdAsync();

        Task<AviaConfiguration> GetSettingConfigurationAsync();

        ClientRoundStatus GetClientRound(string state);

        RoundStatus GetRoundStatus(string state);
    }
}
