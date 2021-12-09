using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.ApiClients.Avia.Models;
using TGS.GameProviders.ApiClients.Avia.Models.Commons;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Extensions;

namespace TGS.GameProviders.ApiClients.Avia
{
    internal class AviaApiClient : IAviaApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<AviaApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public AviaApiClient(
            HttpClient client,
            ILogger<AviaApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public async ValueTask<(bool IsPlayer, decimal? Balance)> BalanceAsync(Uri apiUrl, string authorization, string playerCode)
        {
            var request = new
            {
                UserName = playerCode
            };

            var response = await PostAsync<AviaBaseInfoResponse<AviaBalanceResponse>>(
                apiUrl,
                authorization,
                ToFormData(request),
                AviaAction.Balance).ConfigureAwait(false);

            if (response.Success == 0 )
            {
                return (false, null);
            }

            return (true, response.Success == 1 ? response.Info.Money : null);
        }

        public async ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId)
        {
            var request = new
            {
                UserName = playerCode,
                CateID = cateId,
                MatchID = matchId
            };

            var response = await PostAsync<AviaBaseInfoResponse<AviaLoginResponse>>(
                apiUrl,
                authorization,
                ToFormData(request),
                AviaAction.Login,
                lang
                ).ConfigureAwait(false);

            return response.Success == 1 ? new(response.Info.Url) : null;
        }

        public async ValueTask KickAsync(Uri apiUrl, string authorization, string playerCode)
        {
            var request = new
            {
                UserName = playerCode,
            };

            _ = await PostAsync<AviaBaseResponse>(
                 apiUrl,
                 authorization,
                 ToFormData(request),
                 AviaAction.Logout).ConfigureAwait(false);
        }

        public async ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency)
        {
            var logPrefix = $"{nameof(AviaApiClient).ToLower()}.Register";

            var request = new
            {
                UserName = playerCode,
                Password = password,
                Currency = currency
            };
            var response = await PostAsync<AviaBaseResponse>(
                apiUrl,
                authorization,
                ToFormData(request),
                AviaAction.Register).ConfigureAwait(false);

            return response.Success == 1;
        }

        public async ValueTask<(bool Success, string orderId)> TransferInAsync(Uri apiUrl, string authorization, string playerCode, string currency, string orderId, decimal amount)
        {
            var request = new
            {
                UserName = playerCode,
                Type = "OUT",
                Money = amount,
                ID = orderId,
                Currency = currency
            };

            var response = await PostAsync<AviaBaseInfoResponse<AviaTransferResponse>>(
                 apiUrl,
                 authorization,
                 ToFormData(request),
                 AviaAction.TransferIn).ConfigureAwait(false);

            return response.Success == 1 ? new(true, response.Info.ID.ToString()) : (false, null);
        }

        public async ValueTask<(bool Success, string orderId)> TransferOutAsync(Uri apiUrl, string authorization, string playerCode, string currency, string orderId, decimal amount)
        {
            var request = new
            {
                UserName = playerCode,
                Type = "IN",
                Money = amount,
                ID = orderId,
                Currency = currency
            };

            var response = await PostAsync<AviaBaseInfoResponse<AviaTransferResponse>>(
                 apiUrl,
                 authorization,
                 ToFormData(request),
                 AviaAction.TransferOut).ConfigureAwait(false);

            return response.Success == 1 ? new(true, response.Info.ID.ToString()) : (false, null);
        }

        public async ValueTask<bool> TranslateinfoAsync(Uri apiUrl, string authorization, string orderId)
        {
            var request = new
            {
                ID = orderId
            };

            var response = await PostAsync<AviaBaseInfoResponse<AviaTransferInfoResponse>>(
                 apiUrl,
                 authorization,
                 ToFormData(request),
                 AviaAction.Transferinfo).ConfigureAwait(false);

            return response.Success == 1 && response.Info.Status == AviaTransferInfoStatus.Finish;
        }

        private async Task<TResponse> PostAsync<TResponse>(Uri apiUrl, string authorization, Dictionary<string, string> parameters, AviaAction action, string contentLanguage = "CHN")
        {
            var logPrefix = $"{nameof(AviaApiClient).ToLower()}.{action.ToString()}";
            var requestUri = AviaActionMapper.Mapper.Where(x => x.Key == action).First().Value;
            var sw = Stopwatch.StartNew();
            var responseMessage = default(string);
            var response = default(TResponse);

            var content = new FormUrlEncodedContent(parameters);
            content.Headers.ContentType.CharSet = "utf-8";
            content.Headers.Add("Content-Language", contentLanguage);
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Post;
            requestMessage.RequestUri = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{requestUri}");
            //requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorization);

            requestMessage.Content = content;
            var httpResponseMessage = await _client.SendAsync(requestMessage);
            sw.Stop();
            using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
            {
                responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
            }

            _tgsLog.LogApi(new()
            {
                Ip = string.Empty,
                Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                Prefix = logPrefix,
                ServerName = Environment.MachineName.NoLongerThan(128),
                RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                RequestContent = JsonSerializer.Serialize(parameters),
                ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                ResponseContent = responseMessage,
                ResponseCode = (short)httpResponseMessage.StatusCode,
                ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                RecordedOn = DateTimeOffset.UtcNow
            });

            if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                response = JsonSerializer.Deserialize<TResponse>(responseMessage);
            }
            else
            {
                _logger.LogError(JsonSerializer.Serialize(new
                {
                    Ip = string.Empty,
                    Path = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri.NoLongerThan(512),
                    Method = httpResponseMessage.RequestMessage.Method.ToString().NoLongerThan(8),
                    Prefix = logPrefix,
                    ServerName = Environment.MachineName.NoLongerThan(128),
                    RequestHeaders = httpResponseMessage.RequestMessage.Headers + httpResponseMessage.RequestMessage.Content?.Headers?.ToString(),
                    RequestContent = JsonSerializer.Serialize(parameters),
                    ResponseHeaders = httpResponseMessage.RequestMessage.Headers.ToString(),
                    ResponseContent = responseMessage,
                    ResponseCode = (short)httpResponseMessage.StatusCode,
                    ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
                    RecordedOn = DateTimeOffset.UtcNow
                }));
            }
            return response;
        }

        private Dictionary<string, string> ToFormData<TData>(TData request)
        {
            var value = JsonSerializer.Serialize(request);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(value);
            // return string.Join("&", items.Where(x => x.Value != null).Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
        }
    }
}
