using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

public interface IGameProviderRepository
{
    ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcConfigurationData> GetAwcConfigurationData(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<BTiSportsConfiguration> GetBTiSportsConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<EndorphinaConfiguration> GetEndorphinaConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(
                    Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<string> GetGameProviderCultureCodeAsync(
        Guid gameProviderId,
        string cultureCode,
        CancellationToken cancellationToken = default);

    ValueTask<string> GetGameProviderCurrencyCodeAsync(
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default);

    ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<JiliTransferConfiguration> GetJiliTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<OBetConfiguration> GetOBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SabaTransferConfiguration> GetSabaTransferConfiguration(
            Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SBOTransferConfiguration> GetSBOTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SM01Configuration> GetSM01Configuration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfiguration(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);
}
