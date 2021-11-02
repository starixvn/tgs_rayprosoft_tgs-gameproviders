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
        private const string _gameRecordListPath = "/api/{0}/gamerecordlist";
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

        public async ValueTask<GameRecordListResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_gameRecordListPath, "bg/FishingGame"),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                BrandCode = brandCode,
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_gameRecordListPath, "ky"),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                BrandCode = brandCode,
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                 string.Format(_gameRecordListPath, "ly"),
                 requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> VRGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                BrandCode = brandCode,
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_gameRecordListPath, "vr"),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> WGFMGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                IsSettlement = isSettlement,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_gameRecordListPath, "wgfm"),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> WGVIAGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _serializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_gameRecordListPath, "wgvia"),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

    }
}
