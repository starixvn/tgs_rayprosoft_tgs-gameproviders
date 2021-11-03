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
        private const string _gameURL = "/api/{0}/gameurl";
        private const string _kickPlayerPath = "/api/{0}/KickPlayer";
        private const string _playerBalancePath = "/api/{0}/balance";
        private const string _playerFundChecking = "/api/{0}/PlayerFundChecking";
        private const string _playerFundInPath = "/api/{0}/PlayerFundIn";
        private const string _playerFundOutPath = "/api/{0}/PlayerFundOut";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;

        public GameProvidersClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
        }

        public async ValueTask<Uri?> GameUrlAsync(string gameProviderCode, GameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, PlatformType platformType, string language, string ipAddress, Uri? backUrl)
        {
            var request = new
            {
                GameType = gameType,
                GameId = gameId,
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
                LoginRecordId = loginRecordId,
                PlatformType = platformType,
                Language = language,
                IPAddress = ipAddress,
                BackUrl = backUrl
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_gameURL, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _serializerOptions)?.Url;
        }

        public async ValueTask KickPlayerAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
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

        public async ValueTask<decimal?> PlayerBalanceAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerBalancePath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerBalanceResponse>(contentJson, _serializerOptions)?.Balance;
        }

        public async ValueTask<bool?> PlayerFundCheckingAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
                OrderId = orderId,
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundChecking, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _serializerOptions)?.OrderStatus;
        }

        public async ValueTask<bool?> PlayerFundCheckingByVRAsync(string brandCode, string playerCode, string currencyCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
                OrderId = orderId,
                StartTime = startTime,
                EndTime = endTime
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundChecking, "VR"),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _serializerOptions)?.OrderStatus;
        }

        public async ValueTask<string?> PlayerFundInAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
                OrderId = orderId,
                Amount = amount
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundInPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _serializerOptions)?.OrderId;
        }

        public async ValueTask<string?> PlayerFundOutAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount)
        {
            var request = new
            {
                BrandCode = brandCode,
                PlayerCode = playerCode,
                CurrencyCode = currencyCode,
                OrderId = orderId,
                Amount = amount
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_playerFundOutPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _serializerOptions)?.OrderId;
        }
    }
}
