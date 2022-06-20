using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Repositories
{
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
        public async ValueTask<JiliTransferConfigurationData> GetJiliTransferConfigurationData(Guid gameProviderConfigurationId)
        {
            var data = await _client.GameProviderConfigurationGetAsync(gameProviderConfigurationId).ConfigureAwait(false);
            return data is null ?
                throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfigurationId})")
                : JsonSerializer.Deserialize<JiliTransferConfigurationData>(data.ConfigurationData, _SerializerOptions)!;
        }
    }
}
