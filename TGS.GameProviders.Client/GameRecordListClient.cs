using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    internal class GameRecordListClient : IGameRecordListClient
    {
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
                "​​api​/gamerecordlist​/bg​/fishinggame",
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
                "​​​api​/gamerecordlist​/ky",
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
                "api​/gamerecordlist​/ly",
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
                "​​api​/gamerecordlist​/vr",
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
                "​api​/gamerecordlist​/wgfm",
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
                "​​api​/gamerecordlist​/wgvia",
                  requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<GameRecordListResponse>(contentJson, _serializerOptions);
        }

    }
}
