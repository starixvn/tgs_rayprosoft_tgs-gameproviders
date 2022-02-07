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
using Microsoft.Extensions.Logging;
using Tgs.Logging;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.Models;

namespace TGS.GameProviders.ApiClients.Jili;

internal class JiliApiClient : IJiliApiClient
{
    private const string AlphanumericCharacters = "123456789abcdefghijklmnopqrstuvwxyz";
    private static readonly Random _Random = new Random();
    private readonly HttpClient _client;
    private readonly ILogger<JiliApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public JiliApiClient(
        ILogger<JiliApiClient> logger,
        HttpClient client,
        ITgsLog tgsLog)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
    }

    public ValueTask<JiliGetGameDetailUrlResponse> GetGameDetailUrlAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        string language,
        string externalRoundId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["WagersId"] = externalRoundId,
            ["AgentId"] = agentId
        };

        var keyGen = GetKeyGen(agentId, agentKey);
        var key = GetKey(keyGen, parameters);
        parameters["Key"] = key;
        parameters["Lang"] = language;

        return PostAsync<JiliGetGameDetailUrlResponse>(
            apiUrl,
            "/api1/GetGameDetailUrl",
            parameters,
            "GetGameDetailUrl");
    }

    public ValueTask<JiliKickMemberResponse> KickMemberAsync(
        Uri apiUrl,
        string jiliPlayerId,
        string agentId,
        string agentKey)
    {
        var parameters = new Dictionary<string, string>
        {
            ["Account"] = jiliPlayerId,
            ["AgentId"] = agentId
        };

        var keyGen = GetKeyGen(agentId, agentKey);
        var key = GetKey(keyGen, parameters);
        parameters["Key"] = key;

        return PostAsync<JiliKickMemberResponse>(
            apiUrl,
            "/api1/KickMember",
            parameters,
            "KickMember");
    }

    public ValueTask<JiliLoginWithoutRedirectResponse> LoginWithoutRedirectAsync(
        Uri apiUrl,
        string agentId,
        string agentKey,
        JiliLoginWithoutRedirectRequest request)
    {
        var value = JsonSerializer.Serialize(request);
        var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(value)!;
        parameters["AgentId"] = agentId;

        var keyGen = GetKeyGen(agentId, agentKey);
        var key = GetKey(keyGen, parameters);
        parameters["Key"] = key;

        return GetAsync<JiliLoginWithoutRedirectResponse>(
            apiUrl,
            "/singleWallet/LoginWithoutRedirect",
            parameters,
            "LoginWithoutRedirect");
    }

    private string GenerateString(int length)
    {
        var result = Enumerable
            .Range(1, length)
            .Select(n => AlphanumericCharacters[_Random.Next(0, AlphanumericCharacters.Length)])
            .ToArray();

        return new string(result);
    }

    private async ValueTask<TResponse> GetAsync<TResponse>(Uri apiUrl, string path,
        IEnumerable<KeyValuePair<string, string>> parameters, string action)
    {
        var logPrefix = $"{nameof(JiliApiClient).ToLower()}.{action}";

        var queryString = string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        var uriString = $"{apiUrl.AbsoluteUri.TrimEndSlash()}{path}?{queryString}";

        var sw = Stopwatch.StartNew();

        var httpResponseMessage = await _client.GetAsync(uriString).ConfigureAwait(false);

        sw.Stop();

        var responseMessage = default(string);

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
            RequestContent = string.Empty,
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

    private string GetKey(string keyGen, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var queryString = string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        var value = $"{queryString}{keyGen}";

        var hash = MD5Hash(value);

        return $"{GenerateString(6)}{hash}{GenerateString(6)}";
    }

    private string GetKeyGen(
        string agentId,
        string agentKey)
    {
        var date = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4D)).ToString("yyMMd");
        var value = $"{date}{agentId}{agentKey}";

        return MD5Hash(value);
    }

    private string MD5Hash(string original)
    {
        var data = Encoding.UTF8.GetBytes(original);
        var hash = MD5.HashData(data);

        var sb = new StringBuilder();

        foreach (var t in hash)
        {
            sb.Append(t.ToString("x2"));
        }

        return sb.ToString();
    }

    private async ValueTask<TResponse> PostAsync<TResponse>(Uri apiUrl, string path,
        IEnumerable<KeyValuePair<string, string>> parameters, string action)
    {
        var logPrefix = $"{nameof(JiliApiClient).ToLower()}.{action}";
        var uriString = $"{apiUrl.AbsoluteUri.TrimEndSlash()}{path}";

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
