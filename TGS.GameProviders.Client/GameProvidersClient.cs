using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Client
{
    internal class GameProvidersClient : IGameProvidersClient
    {
        private const string _gameURL = "/api/gameurl/{0}";
        private const string _kickPlayerPath = "/api/KickPlayer/{0}";
        private const string _playerBalancePath = "/api/balance/{0}";
        private const string _playerFundChecking = "/api/PlayerFundChecking/{0}";
        private const string _playerFundInPath = "/api/PlayerFundIn/{0}";
        private const string _playerFundOutPath = "/api/PlayerFundOut/{0}";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;

        public GameProvidersClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
        }

        public async ValueTask<GameUrlResponse?> GameURLAsync(string gameProviderCode, GameUrlRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_gameURL, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask KickPlayerAsync(string gameProviderCode, KickPlayerRequest source)
        {
            var request = new
            {
                BrandCode = source.BrandCode,
                PlayerCode = source.PlayerCode,
                CurrencyCode = source.CurrencyCode,
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_kickPlayerPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(string gameProviderCode, PlayerBalanceRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerBalancePath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerBalanceResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, PlayerFundCheckingRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_gameURL, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, VRPlayerFundCheckingRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   $"/api/PlayerFundChecking/{GameProviderType.VR}",
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<PlayerFundResponse?> PlayerFundInAsync(string gameProviderCode, PlayerFundRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundInPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<PlayerFundResponse?> PlayerFundOutAsync(string gameProviderCode, PlayerFundRequest source)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundOutPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _serializerOptions);
        }
    }
}
