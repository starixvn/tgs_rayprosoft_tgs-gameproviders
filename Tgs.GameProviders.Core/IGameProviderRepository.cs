#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core;

public interface IGameProviderRepository
{
    ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default);

    ValueTask<AVIAGamingConfiguration> GetAVIAGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcAelottoConfiguration> GetAwcAelottoConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcAwsConfiguration> GetAwcAwsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcCockfightConfiguration> GetAwcCockfightConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcFacaiConfiguration> GetAwcFacaiConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcFsConfiguration> GetAwcFsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcKmConfiguration> GetAwcKmConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcLudoConfiguration> GetAwcLudoConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcPgsConfiguration> GetAwcPgsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcRcbConfiguration> GetAwcRcbConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcSgConfiguration> GetAwcSgConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcSpadeConfiguration> GetAwcSpadeConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcVenusConfiguration> GetAwcVenusConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<AwcYlConfiguration> GetAwcYlConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<BTiSportsConfiguration> GetBTiSportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<Cards365TransferConfiguration> GetCards365TransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<EndorphinaConfiguration> GetEndorphinaConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<string?> GetGameProviderCultureCodeAsync(
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

    ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<InplayMatrixESportsConfiguration> GetInplayMatrixESportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<JDBTransferConfiguration> GetJDBTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<JokerConfiguration> GetJokerConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<KaiyuanConfiguration> GetKaiyuanConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<LEGamingConfiguration> GetLEGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<OBetConfiguration> GetOBetConfigurationAsync(
                Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<PinnacleConfiguration> GetPinnacleConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SabaTransferConfiguration> GetSabaTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SBOTransferConfiguration> GetSBOTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<SM01Configuration> GetSM01ConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<WGViaGamingConfiguration> GetWGViaGamingConfigurationAsync(
            Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);

    ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default);
}
