using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.GameProviders.ApiClients.Evolution.Models;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.ApiClients.Evolution
{
    internal sealed class EvolutionApiClient : IEvolutionApiClient
    {
        private readonly HttpClient _client;

        public EvolutionApiClient(
            HttpClient client)

        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ValueTask<EvolutionGameUrlResponse> GetGameUrlAsync(EvolutionConfiguration configuration, EvolutionGameUrlRequest request)
            => PostAsync<EvolutionGameUrlRequest, EvolutionGameUrlResponse>(configuration.Ua2AuthenticationUrl, request, "GetGameUrl");

        public async ValueTask<EvolutionGamHistoryRoundResponse> GetGamHistoryRoundAsync(EvolutionConfiguration configuration, EvolutionGamHistoryRoundRequest request)
        {
            var url = new Uri($"{configuration.ApiUrl}api/gamehistory/v1/players/{request.PlayerCode}/games/{request.RoundId}");
            return await GetAsync<EvolutionGamHistoryRoundResponse>(url, configuration, "GetGamHistoryRound").ConfigureAwait(false);
        }

        private async ValueTask<TResult> GetAsync<TResult>(Uri apiUrl, EvolutionConfiguration configuration, string prefix)
        {
            prefix = $"{GameProviderType.Evolution }.{prefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{configuration.CasinoID}:{configuration.GameHistoryApiToken}"))}");
            var httpResponseMessage = await _client.GetAsync(apiUrl);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return response;
        }

        private Dictionary<string, string> GettHeaders(string casinoKey, string apiToken) => new Dictionary<string, string> {
             { "Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{casinoKey}:{apiToken}"))}" }
        };

        private async ValueTask<TResult> PostAsync<TSource, TResult>(Uri apiUrl, TSource source, string prefix)
        {
            prefix = $"{GameProviderType.Evolution }.{prefix}";

            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResult);
            HttpContent content = new StringContent(JsonSerializer.Serialize(source), Encoding.UTF8, "application/json");
            var httpResponseMessage = await _client.PostAsync(apiUrl, content);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResult>(responseMessage);
            }

            return response;
        }
    }
}
