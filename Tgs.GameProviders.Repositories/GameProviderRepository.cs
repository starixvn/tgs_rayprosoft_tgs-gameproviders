using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories;

internal class GameProviderRepository : IGameProviderRepository
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private readonly IGameCommandClient _client;

    public GameProviderRepository(IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"Game Provider does not exist.({gameProviderId})");

    public async ValueTask<AwcConfigurationData> GetAwcConfigurationData(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var data = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonSerializer.Deserialize<AwcConfigurationData>(data.ConfigurationData, _SerializerOptions)!;
    }

    public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<BTiSportsConfiguration>(
                key: GameProviderConfigurations.BTiSports,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.BTiSports})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<Cards365TransferConfiguration> GetCards365TransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<Cards365TransferConfiguration>(
                key: GameProviderConfigurations.Cards365Transfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.Cards365Transfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<DreamGamingTransferConfiguration> GetDreamGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<DreamGamingTransferConfiguration>(
                key: GameProviderConfigurations.DreamGamingTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.DreamGamingTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<EndorphinaConfiguration> GetEndorphinaConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<EndorphinaConfiguration>(
                key: GameProviderConfigurations.Endorphina,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.Endorphina})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId:
                gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<FaChaiConfiguration> GetFaChaiConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<FaChaiConfiguration>(
                key: GameProviderConfigurations.FaChai,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.FaChai})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId:
                gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var data = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : (data.DesktopEndpoint, data.MobileEndpoint);
    }

    public async ValueTask<string?> GetGameProviderCultureCodeAsync(
        Guid gameProviderId,
        string cultureCode,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetCultureCodeAsync(
            gameProviderId,
            cultureCode,
            cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask<string> GetGameProviderCurrencyCodeAsync(
        Guid gameProviderId,
        string currencyCode,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetCurrencyCodeAsync(
            gameProviderId: gameProviderId,
            currencyCode: currencyCode,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"This game product currency does not exist.({currencyCode})");

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetGameProviderIdAsync(
            gameProviderCode: gameProviderCode,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"Game Provider does not exist.({gameProviderCode})");

    public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<HabaneroTransferConfiguration>(
                key: GameProviderConfigurations.HabaneroTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.HabaneroTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<InplayMatrixESportsConfiguration> GetInplayMatrixESportsConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<InplayMatrixESportsConfiguration>(
                key: GameProviderConfigurations.InplayMatrix_ESports,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.InplayMatrix_ESports})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfigurationAsync(
            Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<JDBTransferConfiguration>(
                key: GameProviderConfigurations.JDBTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JDBTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<JiliTransferConfiguration>(
                key: GameProviderConfigurations.JiliTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JiliTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<JokerConfiguration> GetJokerConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<JokerConfiguration>(
                key: GameProviderConfigurations.Joker,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.Joker})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<LEGamingConfiguration> GetLEGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<LEGamingConfiguration>(
                key: GameProviderConfigurations.LEGaming,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.Joker})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<OBetConfiguration> GetOBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<OBetConfiguration>(
                key: GameProviderConfigurations.OBet,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.OBet})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<PinnacleConfiguration> GetPinnacleConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<PinnacleConfiguration>(
                key: GameProviderConfigurations.Pinnacle,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.Pinnacle})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<PragmaticPlayTransferConfiguration> GetPragmaticPlayTransferConfigurationAsync
        (Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<PragmaticPlayTransferConfiguration>(
                key: GameProviderConfigurations.PragmaticPlayTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.PragmaticPlayTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SabaTransferConfiguration> GetSabaTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<SabaTransferConfiguration>(
                key: GameProviderConfigurations.SabaTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SabaTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<SBOTransferConfiguration>(
                key: GameProviderConfigurations.SBOTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SBOTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SM01Configuration> GetSM01ConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<SM01Configuration>(
                key: GameProviderConfigurations.SM01,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SM01})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<TFGamingTransferConfiguration> GetTFGamingTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<TFGamingTransferConfiguration>(
                key: GameProviderConfigurations.TFGamingTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.TFGamingTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<WGFmGamingConfiguration>(
                key: GameProviderConfigurations.WGFmGaming,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WGFmGaming})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<WGViaGamingConfiguration> GetWGViaGamingConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<WGViaGamingConfiguration>(
                key: GameProviderConfigurations.WGViaGaming,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WGViaGaming})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<WMCasinoTransferConfiguration>(
                key: GameProviderConfigurations.WMCasinoTransfer,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WMCasinoTransfer})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId:
                gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<YeeBetConfiguration> GetYeeBetConfigurationAsync(
        Guid gameProviderConfigurationId,
        CancellationToken cancellationToken = default)
    {
        var settingConfiguration = await _client
            .SettingGetConfigurationAsync<YeeBetConfiguration>(
                key: GameProviderConfigurations.YeeBet,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.YeeBet})");

        var gameProviderConfig = await _client
            .GameProviderConfigurationGetAsync(
                gameProviderConfigId: gameProviderConfigurationId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }
}
