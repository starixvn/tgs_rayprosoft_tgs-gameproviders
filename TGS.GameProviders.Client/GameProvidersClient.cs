using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Client
{
    internal class GameProvidersClient : IGameProvidersClient
    {
        private static readonly string _GameURL = "/api/{0}/gameurl";
        private static readonly string _KickPlayerPath = "/api/{0}/KickPlayer/{1}";
        private static readonly string _PlayerBalancePath = "/api/{0}/balance/{1}";
        private static readonly string _PlayerFundChecking = "/api/{0}/PlayerFundChecking";
        private static readonly string _PlayerFundInPath = "/api/{0}/PlayerFundIn";
        private static readonly string _PlayerFundOutPath = "/api/{0}/PlayerFundOut";
        private static readonly string _RoundDetail = "/api/{0}/RoundDetail/{1}?language={2}";

        private static readonly JsonSerializerOptions? _SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        private readonly HttpClient _httpClient;

        public GameProvidersClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask<Uri?> GameBetConstructUrlAsync(string gameProviderCode, GameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, PlatformType platformType, string language, string ipAddress, Uri? backUrl, string lobbyDomain)
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
                BackUrl = backUrl,
                LobbyDomain = lobbyDomain
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_GameURL, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _SerializerOptions)?.Url;
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_GameURL, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<GameUrlResponse>(contentJson, _SerializerOptions)?.Url;
        }

        public async ValueTask KickPlayerAsync(string gameProviderCode, string playerCode)
        {
            var response = await _httpClient
               .DeleteAsync(
                   string.Format(_KickPlayerPath, gameProviderCode, playerCode))
               .ConfigureAwait(false);            
        }

        public async ValueTask<decimal?> PlayerBalanceAsync(string gameProviderCode, string playerCode)
        {
            var response = await _httpClient
                .GetAsync(
                string.Format(_PlayerBalancePath, gameProviderCode, playerCode))
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<decimal?>(contentJson);
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_PlayerFundChecking, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _SerializerOptions)?.OrderStatus;
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_PlayerFundChecking, "VR"),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return string.IsNullOrEmpty(contentJson) ? false : JsonSerializer.Deserialize<PlayerFundCheckingResponse>(contentJson, _SerializerOptions)?.OrderStatus;
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_PlayerFundInPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _SerializerOptions)?.OrderId;
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

            var requestContent = new StringContent(JsonSerializer.Serialize(request, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                   string.Format(_PlayerFundOutPath, gameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<PlayerFundResponse>(contentJson, _SerializerOptions)?.OrderId;
        }

        public async ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, string language)
        {
            var response = await _httpClient
               .GetAsync(
                   string.Format(_RoundDetail, gameProviderCode, roundId, language))
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<RoundDetailResult>(contentJson, _SerializerOptions);
        }
    }
}
