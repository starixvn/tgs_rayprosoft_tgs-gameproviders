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
using Tgs.GameProviders.ApiClients.Avia.Models;
using Tgs.GameProviders.ApiClients.Avia.Models.Commons;
using Tgs.GameProviders.Core.Avia;
using Tgs.GameProviders.Core.Avia.Models;
using Tgs.GameProviders.Core.Extensions;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.Avia;

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

        var response = await PostAsync<AviaBaseResponse<AviaBalanceResponse>>(
            apiUrl,
            authorization,
            ToFormData(request),
            AviaAction.Balance).ConfigureAwait(false);

        if (response.Success == 0)
        {
            if (response.Info.Error.Contains("NOUSER"))
            {
                return (false, null);
            }
            else
            {
                return (true, null);
            }
        }

        return (true, response.Info.Money);
    }

    public async ValueTask<(bool Success, AviaGameRecordData[]? GameRecords)> GetGameRecordAsync(Uri apiUrl, string authorization, DateTimeOffset startTime, DateTimeOffset endTime, int pageIndex = 1, int pageSize = 20)
    {
        var request = new
        {
            StartAt = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            EndAt = endTime.ToString("yyyy-MM-dd HH:mm:ss"),
            PageIndex = pageIndex.ToString(),
            PageSize = pageSize.ToString()
        };

        var response = await PostAsync<AviaBaseResponse<AviaGameRecordInfoResponse>>(
            apiUrl,
            authorization,
            ToFormData(request),
            AviaAction.GameRecord).ConfigureAwait(false);

        if (response.Success == 0)
        {
            return (false, null);
        }

        return (true, response.Info.List.Length > 0 ? response.Info.List : null);
    }

    public async ValueTask<(bool IsPlayer, Uri? GameUrl)> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId)
    {
        var request = new
        {
            UserName = playerCode,
            CateID = cateId,
            MatchID = matchId
        };

        var response = await PostAsync<AviaBaseResponse<AviaLoginResponse>>(
            apiUrl,
            authorization,
            ToFormData(request),
            AviaAction.Login,
            lang
            ).ConfigureAwait(false);

        if (response.Success == 0)
        {
            if (response.Info.Error.Contains("NOUSER"))
            {
                return (false, null);
            }
            else
            {
                return (true, null);
            }
        }

        return (true, new(response.Info.Url));
    }

    public async ValueTask KickAsync(Uri apiUrl, string authorization, string playerCode)
    {
        var request = new
        {
            UserName = playerCode,
        };

        _ = await PostAsync<AviaBaseResponse<AviaErrorResponse>>(
             apiUrl,
             authorization,
             ToFormData(request),
             AviaAction.Logout).ConfigureAwait(false);
    }

    public async ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency)
    {
        var request = new
        {
            UserName = playerCode,
            Password = password,
            Currency = currency
        };
        var response = await PostAsync<AviaBaseResponse<AviaErrorResponse>>(
            apiUrl,
            authorization,
            ToFormData(request),
            AviaAction.Register).ConfigureAwait(false);

        return response.Success == 1;
    }

    public async ValueTask<(bool Success, string orderId)> TransferInAsync(Uri apiUrl, string authorization, string playerCode, string currency, decimal amount)
    {
        var orderId = $"{GetDateTimeOffsetNowTimeZone8():yyyyMMddHHmmssfff}{playerCode}";
        var request = new
        {
            UserName = playerCode,
            Type = "OUT",
            Money = amount.ToString("#0.00"),
            ID = orderId,
            Currency = currency
        };

        var response = await PostAsync<AviaBaseResponse<AviaTransferResponse>>(
             apiUrl,
             authorization,
             ToFormData(request),
             AviaAction.TransferIn).ConfigureAwait(false);

        return response.Success == 1 ? new(true, response.Info.OrderID.ToString()) : (false, null);
    }

    public async ValueTask<(bool Success, string orderId)> TransferOutAsync(Uri apiUrl, string authorization, string playerCode, string currency, decimal amount)
    {
        var orderId = $"{GetDateTimeOffsetNowTimeZone8():yyyyMMddHHmmssfff}{playerCode}";
        var request = new
        {
            UserName = playerCode,
            Type = "IN",
            Money = amount.ToString("#0.00"),
            ID = orderId,
            Currency = currency
        };

        var response = await PostAsync<AviaBaseResponse<AviaTransferResponse>>(
             apiUrl,
             authorization,
             ToFormData(request),
             AviaAction.TransferOut).ConfigureAwait(false);

        return response.Success == 1 ? new(true, response.Info.OrderID.ToString()) : (false, null);
    }

    public async ValueTask<bool> TranslateinfoAsync(Uri apiUrl, string authorization, string orderId)
    {
        var request = new
        {
            ID = orderId
        };

        var response = await PostAsync<AviaBaseResponse<AviaTransferInfoResponse>>(
             apiUrl,
             authorization,
             ToFormData(request),
             AviaAction.Transferinfo).ConfigureAwait(false);

        if (response.Success == 0)
        {
            return false;
        }

        _ = AviaMapper.AviaTransferInfoStatusMapper.TryGetValue(response.Info.Status, out AviaTransferInfoStatus transferInfoStatus);

        return response.Success == 1 && transferInfoStatus == AviaTransferInfoStatus.Finish;
    }

    private static Dictionary<string, string> ToFormData<TData>(TData request)
    {
        var value = JsonSerializer.Serialize(request);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(value);
    }

    private DateTimeOffset GetDateTimeOffsetNowTimeZone8()
        => DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));

    private async Task<TResponse> PostAsync<TResponse>(Uri apiUrl, string authorization, Dictionary<string, string> parameters, AviaAction action, string contentLanguage = "CHN")
    {
        var logPrefix = $"{nameof(AviaApiClient).ToLower()}.{action}";
        var requestUri = AviaMapper.AviaActionMapper.Where(x => x.Key == action).First().Value;
        var sw = Stopwatch.StartNew();

        var content = new FormUrlEncodedContent(parameters);
        content.Headers.ContentType.CharSet = "utf-8";
        content.Headers.Add("Content-Language", contentLanguage);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{apiUrl.Scheme}://{apiUrl.Host}{requestUri}")
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorization);

        requestMessage.Content = content;
        var httpResponseMessage = await _client.SendAsync(requestMessage);
        sw.Stop();

        var responseMessage = default(string);
        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync().Result))
        {
            responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
        }

        var apiLogEvent = new ApiLogEvent
        {
            Ip = string.Empty,
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = (await content.ReadAsStringAsync().ConfigureAwait(false)).NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };
        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) && httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage);
        }

        return default;
    }
}
