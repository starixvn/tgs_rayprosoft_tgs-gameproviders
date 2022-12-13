using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

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

    private readonly IGameProvidersQueryServerClient _client;

    public GameProviderRepository(IGameProvidersQueryServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameProviderModel> GetAsync(Guid gameProviderId)
        => await _client.GameProviderGetAsync(gameProviderId).ConfigureAwait(false)
        ?? throw new Exception($"Game Provider does not exist.({gameProviderId})");

    public async ValueTask<AwcConfigurationData> GetAwcConfigurationData(Guid gameProviderConfigurationId)
    {
        var data = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonSerializer.Deserialize<AwcConfigurationData>(data.ConfigurationData, _SerializerOptions)!;
    }

    public async ValueTask<BTiSportsConfiguration> GetBTiSportsConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<BTiSportsConfiguration>(GameProviderConfigurations.BTiSports).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.BTiSports})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<(string DesktopEndpoint, string MobileEndpoint)> GetGameEndpointsAsync(Guid gameProviderConfigurationId)
    {
        var data = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : (data.DesktopEndpoint, data.MobileEndpoint);
    }

    public ValueTask<string?> GetGameProviderCultureCodeAsync(Guid gameProviderId, string cultureCode)
        => _client.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode);

    public ValueTask<string?> GetGameProviderCurrencyCodeAsync(Guid gameProviderId, string currencyCode)
        => _client.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode);

    public async ValueTask<Guid> GetGameProviderIdAsync(string gameProviderCode)
        => await _client.GameProviderGetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false)
        ?? throw new Exception($"Game Provider does not exist.({gameProviderCode})");

    public async ValueTask<HabaneroTransferConfiguration> GetHabaneroTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<HabaneroTransferConfiguration>(GameProviderConfigurations.HabaneroTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.HabaneroTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
            ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<JDBTransferConfiguration>(GameProviderConfigurations.JDBTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JDBTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<JiliTransferConfiguration> GetJiliTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<JiliTransferConfiguration>(GameProviderConfigurations.JiliTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JiliTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SabaTransferConfiguration> GetSabaTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<SabaTransferConfiguration>(GameProviderConfigurations.SabaTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SabaTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SBOTransferConfiguration> GetSBOTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<SBOTransferConfiguration>(GameProviderConfigurations.SBOTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SBOTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<WGFmGamingConfiguration>(GameProviderConfigurations.WGSports).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WGSports})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<WMCasinoTransferConfiguration> GetWMCasinoTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<WMCasinoTransferConfiguration>(GameProviderConfigurations.WMCasinoTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WMCasinoTransfer})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? settingConfiguration
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }

    public async ValueTask<SM01Configuration> GetSM01Configuration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationAsync<SM01Configuration>(GameProviderConfigurations.SM01).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.SM01})");

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        return string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData)
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonMergeUtils.MergeModel(settingConfiguration, gameProviderConfig.ConfigurationData, _SerializerOptions);
    }
}
