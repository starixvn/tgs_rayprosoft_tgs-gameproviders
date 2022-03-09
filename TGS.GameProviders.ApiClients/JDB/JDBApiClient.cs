using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.JDB;
using TGS.GameProviders.Core.JDB.Models;

namespace TGS.GameProviders.ApiClients.JDB;

internal class JDBApiClient : IJDBApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<JDBApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public JDBApiClient(
        ILogger<JDBApiClient> logger,
        HttpClient client,
        ITgsLog tgsLog)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
    }

    public ValueTask<JDBBetHistoryResponse> GetBetHistoryUrlAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBBetHistoryRequest request)
    {
        var json = JsonSerializer.Serialize(request);

        var encryptedString = AesEncrypt(aesIv, aesKey, json);

        var parameters = new Dictionary<string, string>
        {
            ["dc"] = clientDomain,
            ["x"] = encryptedString
        };

        return PostAsync<JDBBetHistoryResponse>(
            apiUrl,
            "/apiRequest.do",
            parameters,
            "BetHistory");
    }

    public ValueTask<JDBGetTokenResponse> GetTokenAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBGetTokenRequest request)
    {
        var json = JsonSerializer.Serialize(request);

        var encryptedString = AesEncrypt(aesIv, aesKey, json);

        var parameters = new Dictionary<string, string>
        {
            ["dc"] = clientDomain,
            ["x"] = encryptedString
        };

        return PostAsync<JDBGetTokenResponse>(
            apiUrl,
            "/apiRequest.do",
            parameters,
            "GetToken");
    }

    public ValueTask<JDBLogoutResponse> LogoutAsync(
        Uri apiUrl,
        string clientDomain,
        string aesIv,
        string aesKey,
        JDBLogoutRequest request)
    {
        var json = JsonSerializer.Serialize(request);

        var encryptedString = AesEncrypt(aesIv, aesKey, json);

        var parameters = new Dictionary<string, string>
        {
            ["dc"] = clientDomain,
            ["x"] = encryptedString
        };

        return PostAsync<JDBLogoutResponse>(
            apiUrl,
            "/apiRequest.do",
            parameters,
            "Logout");
    }

    private string AesEncrypt(string aesIv, string aesKey, string value)
    {
        using var aes = Aes.Create();
        aes.IV = Encoding.UTF8.GetBytes(aesIv);
        aes.Key = Encoding.UTF8.GetBytes(aesKey);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var ct = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

        var bytes = Encoding.UTF8.GetBytes(value);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();

        return Convert.ToBase64String(ms.ToArray());
    }

    private async ValueTask<TResponse> PostAsync<TResponse>(
        Uri apiUrl,
        string path,
        IEnumerable<KeyValuePair<string, string>> parameters,
        string action)
    {
        var logPrefix = $"{nameof(JDBApiClient).ToLower()}.{action}";
        var uriString = $"{apiUrl.AbsoluteUri.TrimEnd('/')}{path}";

        var sw = Stopwatch.StartNew();

        var content = new FormUrlEncodedContent(parameters);
        var httpResponseMessage = await _client.PostAsync(uriString, content).ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);

        var requestMessage = await content.ReadAsStringAsync().ConfigureAwait(false);

        var stream = await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using (var sr = new StreamReader(stream))
        {
            responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        var apiLogEvent = new ApiLogEvent
        {
            Ip = string.Empty,
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders =
                httpResponseMessage.RequestMessage?.Headers +
                httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = requestMessage,
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage,
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);

        if (!string.IsNullOrEmpty(responseMessage) &&
            httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            return JsonSerializer.Deserialize<TResponse>(responseMessage);
        }

        _logger.LogError(JsonSerializer.Serialize(apiLogEvent));

        return default;
    }
}
