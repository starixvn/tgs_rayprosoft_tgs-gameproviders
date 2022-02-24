using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace TGS.GameProviders.Client
{
    internal class GameRecordListClient : IGameRecordListClient
    {
        private static readonly string _GameRecordListPath = "/api/{0}/gamerecordlist";

        private static readonly JsonSerializerOptions? _SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        private readonly HttpClient _httpClient;

        public GameRecordListClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async ValueTask<GameRecordResponse?> AviaGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                BrandCode = brandCode,
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                $"/api/{GameProviderType.Avia}/GameRecordList",
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                $"/api/{GameProviderType.BigGaming}/FishingGameRecordList",
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                BrandCode = brandCode,
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_GameRecordListPath, GameProviderType.KY),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                BrandCode = brandCode,
                CurrencyCode = currencyCode,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                 string.Format(_GameRecordListPath, GameProviderType.LY),
                 requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
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
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_GameRecordListPath, GameProviderType.VR),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> WGHorseGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows)
        {
            var source = new
            {
                StartTime = startTime,
                EndTime = endTime,
                Page = page,
                Rows = rows
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_GameRecordListPath, GameProviderType.WGHorse),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }

        public async ValueTask<GameRecordResponse?> WGSportsGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var source = new
            {
                IsSettlement = isSettlement,
                StartTime = startTime,
                EndTime = endTime
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
               .PostAsync(
                string.Format(_GameRecordListPath, GameProviderType.WGSports),
                requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordResponse>(contentJson, _SerializerOptions);
        }
    }
}
