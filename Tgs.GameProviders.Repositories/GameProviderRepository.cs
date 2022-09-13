using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
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
    public async ValueTask<JDBTransferConfiguration> GetJDBTransferConfiguration(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationStringAsync(GameProviderConfigurations.JDBTransfer).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JDBTransfer})");

        var settingConfigurationJson = JsonNode.Parse(settingConfiguration);

        if (settingConfigurationJson == null)
        {
            throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.JDBTransfer})");
        }

        var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                  ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

        var configurationDataJson = JsonNode.Parse(gameProviderConfig.ConfigurationData);

        return configurationDataJson is null
            ? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : new JDBTransferConfiguration()
            {
                ApiUrl = new(configurationDataJson["ApiUrl"]?.ToString()
                            ?? settingConfigurationJson["ApiUrl"]?.ToString()
                            ?? throw new Exception($"{GameProviderConfigurations.JDBTransfer} Required parameters(ApiUrl)")),
                Domain = configurationDataJson["Domain"]?.ToString()
                            ?? settingConfigurationJson["Domain"]?.ToString()
                            ?? throw new Exception($"{GameProviderConfigurations.JDBTransfer} Required parameters(Domain)"),
                Agent = configurationDataJson["Agent"]?.ToString()
                            ?? throw new Exception($"{GameProviderConfigurations.JDBTransfer} Required parameters(Agent)"),
                AesIV = configurationDataJson["AesIV"]?.ToString()
                            ?? settingConfigurationJson["AesIV"]?.ToString()
                            ?? throw new Exception($"{GameProviderConfigurations.JDBTransfer} Required parameters(AesIV)"),
                AesKey = configurationDataJson["AesKey"]?.ToString()
                            ?? settingConfigurationJson["AesKey"]?.ToString()
                            ?? throw new Exception($"{GameProviderConfigurations.JDBTransfer} Required parameters(AesKey)")
            };
    }

    public async ValueTask<JiliTransferConfigurationData> GetJiliTransferConfigurationData(Guid gameProviderConfigurationId)
    {
        var data = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonSerializer.Deserialize<JiliTransferConfigurationData>(data.ConfigurationData, _SerializerOptions)!;
    }

    public async ValueTask<WGFmGamingConfiguration> GetWGFmGamingConfigurationData(Guid gameProviderConfigurationId)
    {
        var settingConfiguration = await _client.SettingGetConfigurationStringAsync(GameProviderConfigurations.WGSports).ConfigureAwait(false)
            ?? throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WGSports})");

        var settingConfigurationJson = JsonNode.Parse(settingConfiguration);

        if (settingConfigurationJson == null)
        {
            throw new Exception($"This game product setting does not exist.({GameProviderConfigurations.WGSports})");
        }

        var amountFormat = settingConfigurationJson["AmountFormat"]!.ToString();
        var amountMaximum = decimal.Parse(settingConfigurationJson["AmountMaximum"]!.ToString());


        var apiUrl = settingConfigurationJson["ApiUrl"]?.ToString()
            ?? throw new Exception($"{GameProviderConfigurations.WGSports} Required parameters(ApiUrl)");

        var partnerCode = settingConfigurationJson["PartnerCode"]?.ToString()
            ?? throw new Exception($"{GameProviderConfigurations.WGSports} Required parameters(PartnerCode)");

        var partnerKey = settingConfigurationJson["PartnerKey"]?.ToString()
            ?? throw new Exception($"{GameProviderConfigurations.WGSports} Required parameters(PartnerKey)");

        var partneIV = settingConfigurationJson["IV"]?.ToString()
            ?? throw new Exception($"{GameProviderConfigurations.WGSports} Required parameters(IV)");

        var version = settingConfigurationJson["Version"]?.ToString()
           ?? throw new Exception($"{GameProviderConfigurations.WGSports} Required parameters(Version)");

        var configuration = new WGFmGamingConfiguration()
        {
            AmountFormat = amountFormat,
            AmountMaximum = amountMaximum,
            ApiUrl = new Uri(apiUrl),
            PartnerCode = partnerCode,
            PartnerKey = partnerKey,
            IV = partneIV,
            Version = int.Parse(version)
        };

        try
        {
            var gameProviderConfig = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false)
                      ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");

            if (string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData))
            {
                return configuration;
            }

            var configurationDataJson = JsonNode.Parse(gameProviderConfig.ConfigurationData);

            return configurationDataJson is null
                ? configuration
                : new WGFmGamingConfiguration()
                {
                    AmountFormat = amountFormat,
                    AmountMaximum = amountMaximum,
                    ApiUrl = new Uri((configurationDataJson["ApiUrl"]?.ToString() ?? apiUrl)),
                    PartnerCode = (configurationDataJson["PartnerCode"]?.ToString() ?? partnerCode),
                    PartnerKey = (configurationDataJson["PartnerKey"]?.ToString() ?? partnerKey),
                    IV = (configurationDataJson["IV"]?.ToString() ?? partneIV),
                    Version = int.Parse(configurationDataJson["Version"]?.ToString() ?? version.ToString())
                };
        }
        catch
        {
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})");
        }
    }

    public async ValueTask<WMCasinoTransferConfigurationData> GetWMCasinoTransferConfigurationData(Guid gameProviderConfigurationId)
    {
        var data = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false);
        return data is null ?
            throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
            : JsonSerializer.Deserialize<WMCasinoTransferConfigurationData>(data.ConfigurationData, _SerializerOptions)!;
    }
}
