using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    internal class GameRecordListClient : IGameRecordListClient
    {
        private const string _gameRecordListPath = "/api/{0}/gamerecordlist";
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;

        public GameRecordListClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
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

        public async ValueTask<GameRecordListResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                BrandCode = brandCode,
                IsUpdateTime = isUpdateTime,
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
