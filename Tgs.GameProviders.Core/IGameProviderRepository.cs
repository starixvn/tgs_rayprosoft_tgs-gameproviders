#nullable enable

using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core;

public interface IGameProviderRepository
{
    ValueTask<GameProviderModel> GetAsync(Guid gameProviderId);

    ValueTask<AwcConfigurationData> GetAwcConfigurationData(Guid gameProviderConfigurationId);

    ValueTask<BTiSportsConfiguration> GetBTiSportsConfiguration(Guid gameProviderConfigurationId);

    ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId);

    ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode);

    ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

    ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode);

    ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<JiliTransferConfiguration> GetJiliTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<SabaTransferConfiguration> GetSabaTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<SBOTransferConfiguration> GetSBOTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<SM01Configuration> GetSM01Configuration(Guid gameProviderConfigurationId);

    ValueTask<TFGamingTransferConfiguration> GetTFGamgingTransferConfiguration(Guid gameProviderConfigurationId);

    ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfiguration(Guid gameProviderConfigurationId);

    ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfiguration(Guid gameProviderConfigurationId);
}
