using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.AllBet;
using Tgs.GameProviders.Core.AllBet.Models;
using Tgs.GameProviders.Core.Extensions;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.AllBet;

internal class AllBetApiClient : IAllBetApiClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<AllBetApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public AllBetApiClient(
        ILogger<AllBetApiClient> logger,
        HttpClient client,
        ITgsLog tgsLog)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false
        };
    }

    public ValueTask<AllBetCheckOrCreateMemberResponse> CheckOrCreateMemberAsync(
        Uri apiUrl,
        string propertyId,
        string apiKey,
        AllBetCheckOrCreateMemberRequest request)
    {
        var method = HttpMethod.Post;
        var path = "/CheckOrCreate";

        var (headers, contentHeaders) =
            GetHeaders(method, propertyId, apiKey, path, request);

        return SendAsync<AllBetCheckOrCreateMemberRequest, AllBetCheckOrCreateMemberResponse>(
            method,
            apiUrl,
            path,
            headers,
            contentHeaders,
            request,
            "CheckOrCreateMember");
    }

    public ValueTask<AllBetLoginResponse> LoginAsync(
        Uri apiUrl,
        string propertyId,
        string apiKey,
        AllBetLoginRequest request)
    {
        var method = HttpMethod.Post;
        var path = "/Login";

        var (headers, contentHeaders) =
            GetHeaders(method, propertyId, apiKey, path, request);

        return SendAsync<AllBetLoginRequest, AllBetLoginResponse>(
            method,
            apiUrl,
            path,
            headers,
            contentHeaders,
            request,
            "Login");
    }

    public ValueTask<AllBetLogoutResponse> LogoutAsync(Uri apiUrl, string propertyId, string apiKey, AllBetLogoutRequest request)
    {
        var method = HttpMethod.Post;
        var path = "/Logout";

        var (headers, contentHeaders) =
            GetHeaders(method, propertyId, apiKey, path, request);

        return SendAsync<AllBetLogoutRequest, AllBetLogoutResponse>(
            method,
            apiUrl,
            path,
            headers,
            contentHeaders,
            request,
            "Logout");
    }

    private (IReadOnlyDictionary<string, string> Headers, IReadOnlyDictionary<string, string> ContentHeaders)
        GetHeaders<TRequest>(HttpMethod method, string propertyId, string apiKey, string path, TRequest request)
    {
        var body = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        var md5 = Md5Hash(body);
        var contentType = "application/json";

        // RFC 1123 pattern
        var date = DateTime.Now.ToUniversalTime().ToString("R", new DateTimeFormatInfo());

        var stringToSign = string.Join("\n", method.ToString(), md5, contentType, date, path);
        var encrypted = Sha1Hash(apiKey, stringToSign);

        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"AB {propertyId}:{encrypted}",
            ["Date"] = date
        };

        var contentHeaders = new Dictionary<string, string>
        {
            ["Content-MD5"] = md5,
            ["Content-Type"] = contentType
        };

        return (headers, contentHeaders);
    }

    private string Md5Hash(string original)
    {
        using var hasher = MD5.Create();
        var data = Encoding.UTF8.GetBytes(original);
        var hash = hasher.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }

    private async ValueTask<TResponse> SendAsync<TRequest, TResponse>(
                        HttpMethod method,
            Uri apiUrl,
            string path,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> contentHeaders,
            TRequest request,
            string action)
    {
        var logPrefix = $"{nameof(AllBetApiClient).ToLower()}.{action}";

        var uriString = $"{apiUrl.AbsoluteUri.TrimEnd('/')}{path}";

        var sw = Stopwatch.StartNew();

        var data = JsonSerializer.Serialize(request);

        var httpRequestMessage = new HttpRequestMessage(method, uriString)
        {
            Content = new StringContent(data, Encoding.UTF8, "application/json")
        };

        foreach (var (key, value) in headers)
        {
            httpRequestMessage.Headers.Remove(key);
            httpRequestMessage.Headers.Add(key, value);
        }

        foreach (var (key, value) in contentHeaders)
        {
            httpRequestMessage.Content.Headers.Remove(key);
            httpRequestMessage.Content.Headers.Add(key, value);
        }

        var httpResponseMessage = await _client.SendAsync(httpRequestMessage).ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);

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
            RequestContent = data.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };
        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation(JsonSerializer.Serialize(apiLogEvent));

        if (!string.IsNullOrEmpty(responseMessage) &&
            httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage);
        }

        return default;
    }

    private string Sha1Hash(string apiKey, string sign)
    {
        using var hasher = new HMACSHA1(Convert.FromBase64String(apiKey));
        var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(sign));
        return Convert.ToBase64String(hash); ;
    }
}
