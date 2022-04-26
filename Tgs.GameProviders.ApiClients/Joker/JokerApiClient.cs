#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.Joker;
using Tgs.GameProviders.Core.Joker.Models;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.Joker;

internal class JokerApiClient : IJokerApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<JokerApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public JokerApiClient(
        HttpClient client,
        ITgsLog tgsLog,
        ILogger<JokerApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<bool> CreateUserAsync(Uri apiUrl, string playerCode, string appId, string secretKey)
    {
        var request = new
        {
            Method = "CU",
            Username = playerCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(request);
        var signature = GenerateSignature(document, secretKey);

        var (_, statusCode, _) = await PostAsync(
            apiUrl,
            appId,
            signature,
            document,
            "CreateUser")
            .ConfigureAwait(false);

        return statusCode switch
        {
            HttpStatusCode.OK or HttpStatusCode.Created => true,
            _ => throw new Exception($"Joker returns status code: {(int)statusCode}")
        };
    }

    public async ValueTask<bool> FundInAsync(
        Uri apiUrl,
        string playerCode,
        string orderId,
        decimal amount,
        string appId,
        string secretKey)
    {
        var parameters = new Dictionary<string, object>
        {
            ["Method"] = "TC",
            ["Username"] = playerCode,
            ["RequestID"] = orderId,
            ["Amount"] = -amount,
            ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(parameters);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, _, _) = await PostAsync(
                apiUrl,
                appId,
                signature,
                document,
                "FundIn")
            .ConfigureAwait(false);

        return isSuccessStatusCode;
    }

    public async ValueTask<bool> FundOrderCheckingAsync(Uri apiUrl, string orderId, string appId, string secretKey)
    {
        var parameters = new Dictionary<string, object>
        {
            ["Method"] = "TCH",
            ["RequestID"] = orderId,
            ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(parameters);
        var signature = GenerateSignature(document, secretKey);

        var (_, statusCode, _) = await PostAsync(
                apiUrl,
                appId,
                signature,
                document,
                "FundOrderChecking")
            .ConfigureAwait(false);

        return statusCode switch
        {
            HttpStatusCode.OK => true,
            _ => throw new Exception($"Joker returns status code: {(int)statusCode}")
        };
    }

    public async ValueTask<bool> FundOutAsync(
        Uri apiUrl,
        string playerCode,
        string orderId,
        decimal amount,
        string appId,
        string secretKey)
    {
        var parameters = new Dictionary<string, object>
        {
            ["Method"] = "TC",
            ["Username"] = playerCode,
            ["RequestID"] = orderId,
            ["Amount"] = amount,
            ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(parameters);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, _, _) = await PostAsync(
            apiUrl,
            appId,
            signature,
            document,
            "FundOut")
            .ConfigureAwait(false);

        return isSuccessStatusCode;
    }

    public async ValueTask<(bool? IsPlayerExist, decimal? Balance)> GetBalanceAsync(
        Uri apiUrl,
        string playerCode,
        string appId,
        string secretKey)
    {
        var request = new
        {
            Method = "GC",
            Username = playerCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(request);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, statusCode, content) = await PostAsync(
            apiUrl,
            appId,
            signature,
            document,
            "GetBalance")
            .ConfigureAwait(false);

        if (!isSuccessStatusCode)
        {
            return statusCode == HttpStatusCode.NotFound
                ? (false, null)
                : (null, null);
        }

        var balance = content?.RootElement.GetProperty("Credit").GetDecimal();
        return (true, balance);
    }

    public async ValueTask<string?> GetGameDetailUrlAsync(
        Uri apiUrl,
        string language,
        string externalRoundId,
        JokerDataType dataType,
        string appId,
        string secretKey)
    {
        var parameters = new Dictionary<string, object>
        {
            ["Method"] = "History",
            ["OCode"] = externalRoundId,
            ["Language"] = language,
            ["Type"] = dataType.ToString(),
            ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(parameters);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, _, content) = await PostAsync(
                apiUrl,
                appId,
                signature,
                document,
                "GetGameDetailUrl")
            .ConfigureAwait(false); ;

        return !isSuccessStatusCode ? null : content?.RootElement.GetProperty("Url").GetString();
    }

    public async ValueTask<(bool Success, JokerGameRecordInfoResponse? Response)> GetGameRecordAsync(
        Uri apiUrl,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        string? nextId,
        string appId,
        string secretKey)
    {
        var eightHourOffset = TimeSpan.FromHours(8);

        var parameters = new Dictionary<string, object>
        {
            ["Method"] = "TSM",
            ["StartDate"] = startTime.ToOffset(eightHourOffset).ToString("yyyy-MM-dd HH:mm"),
            ["EndDate"] = endTime.ToOffset(eightHourOffset).ToString("yyyy-MM-dd HH:mm"),
            ["NextId"] = nextId ?? string.Empty,
            ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(parameters);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, _, content) = await PostAsync(
                apiUrl,
                appId,
                signature,
                document,
                "GetGameRecord")
            .ConfigureAwait(false); ;

        return (isSuccessStatusCode, content?.Deserialize<JokerGameRecordInfoResponse>());
    }

    public async ValueTask<string?> GetGameTokenAsync(Uri apiUrl, string playerCode, string appId, string secretKey)
    {
        var request = new
        {
            Method = "PLAY",
            Username = playerCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(request);
        var signature = GenerateSignature(document, secretKey);

        var (isSuccessStatusCode, _, content) = await PostAsync(
                apiUrl,
                appId,
                signature,
                document,
                "GameToken")
            .ConfigureAwait(false);

        return isSuccessStatusCode
            ? content?.RootElement.GetProperty("Token").GetString()
            : null;
    }

    public async ValueTask KickPlayerAsync(Uri apiUrl, string playerCode, string appId, string secretKey)
    {
        var request = new
        {
            Method = "SO",
            Username = playerCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(request);
        var signature = GenerateSignature(document, secretKey);

        _ = await PostAsync(apiUrl, appId, signature, document, "KickPlayer")
            .ConfigureAwait(false); ;
    }

    private static string GenerateSignature(JsonDocument document, string secretKey)
    {
        var parameters = document.Deserialize<SortedDictionary<string, object>>()!;

        var values = parameters.Select(p => $"{p.Key}={p.Value}");
        var original = string.Join("&", values);

        using var sha = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(original));
        return Convert.ToBase64String(hash);
    }

    private async ValueTask<(bool IsSuccessStatusCode, HttpStatusCode HttpStatusCode, JsonDocument? Content)> PostAsync(
        Uri apiUrl,
        string appId,
        string signature,
        JsonDocument request,
        string action)
    {
        var logPrefix = $"{nameof(JokerApiClient).ToLower()}.{action}";

        var sw = Stopwatch.StartNew();

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("appid", appId);
        queryStrings.Add("signature", signature);

        var uri = $"{apiUrl.OriginalString}?{queryStrings}";

        var requestMessage = JsonSerializer.Serialize(request);
        var content = new StringContent(requestMessage,
            Encoding.UTF8,
            "application/json");

        var httpResponseMessage = await _client.PostAsync(uri, content).ConfigureAwait(false);

        sw.Stop();

        string responseMessage;

        var stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using (var sr = new StreamReader(stream))
        {
            responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        var apiLogEvent = new ApiLogEvent
        {
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = requestMessage.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        var responseDocument = !string.IsNullOrEmpty(responseMessage) &&
               httpResponseMessage.IsSuccessStatusCode
            ? JsonDocument.Parse(responseMessage)
            : null;

        return (httpResponseMessage.IsSuccessStatusCode, httpResponseMessage.StatusCode, responseDocument);
    }
}