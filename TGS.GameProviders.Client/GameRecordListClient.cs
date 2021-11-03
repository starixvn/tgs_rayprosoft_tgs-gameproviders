using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

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
                $"/api/{GameProviderType.BigGaming}/FishingGameRecordList",
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
                string.Format(_gameRecordListPath, GameProviderType.KY),
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
                 string.Format(_gameRecordListPath, GameProviderType.LY),
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
                string.Format(_gameRecordListPath, GameProviderType.VR),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> WGSportsGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime)
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
                string.Format(_gameRecordListPath, GameProviderType.WGSports),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

        public async ValueTask<GameRecordListResponse?> WGHorseGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
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
                string.Format(_gameRecordListPath, GameProviderType.WGHorse),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }
    }
}
