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
    internal class GameProvidersClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _serializerOptions;
        private const string _kickPlayerPath = "/api/KickPlayer/{0}";
        private const string _balancePath = "/api/balance/{0}";
        public GameProvidersClient(HttpClient httpClient, JsonSerializerOptions? serializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = serializerOptions;
        }

        public async ValueTask KickPlayer(KickPlayerRequest source)
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
                   string.Format(_kickPlayerPath, source.GameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<PlayerBalanceResponse> PlayerBalance(PlayerBalanceRequest source)
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
                   string.Format(_balancePath, source.GameProviderCode),
                   requestContent)
               .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<PlayerBalanceResponse>(contentJson, _serializerOptions);
        }
    }
}
