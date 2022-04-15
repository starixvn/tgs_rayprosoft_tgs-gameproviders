#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.Extensions;
using Tgs.GameProviders.Core.Joker;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.Joker;

internal class JokerApiClient : IJokerApiClient
{
    private readonly HttpClient _client;
    private readonly ITgsLog _tgsLog;
    private readonly ILogger<JokerApiClient> _logger;

    public JokerApiClient(
        HttpClient client,
        ITgsLog tgsLog,
        ILogger<JokerApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        var response = await PostAsync(apiUrl, appId, signature, document, "GameToken");

        return response?.RootElement.GetProperty("Token").GetString();
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

        var response = await PostAsync(apiUrl, appId, signature, document, "CreateUser");

        var status = response?.RootElement.GetProperty("Status").GetString();

        return status is "OK" or "Created";
    }

    public async ValueTask<decimal?> GetBalanceAsync(Uri apiUrl, string playerCode, string appId, string secretKey)
    {
        var request = new
        {
            Method = "GC",
            Username = playerCode,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var document = JsonSerializer.SerializeToDocument(request);
        var signature = GenerateSignature(document, secretKey);

        var response = await PostAsync(apiUrl, appId, signature, document, "GetBalance");

        return response?.RootElement.GetProperty("Credit").GetDecimal();
    }

    private async ValueTask<JsonDocument?> PostAsync(
        Uri apiUrl,
        string appId,
        string signature,
        JsonDocument request,
        string action)
    {
        var logPrefix = $"{nameof(JokerApiClient).ToLower()}.{action}";

        var sw = Stopwatch.StartNew();

        var uriBuilder = new UriBuilder(apiUrl)
        {
            Query = $"appid={appId}&signature={signature}"
        };

        var requestMessage = JsonSerializer.Serialize(request);
        var content = new StringContent(requestMessage,
            Encoding.UTF8,
            "application/json");

        var httpResponseMessage = await _client.PostAsync(uriBuilder.ToString(), content).ConfigureAwait(false);

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
            RequestContent = requestMessage.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        Console.WriteLine("Signature: {0}", signature);
        Console.WriteLine("Request Message: {0}", requestMessage);
        Console.WriteLine("Response Status Code: {0}", httpResponseMessage.StatusCode);
        Console.WriteLine("Response Message: {0}", responseMessage);

        return !string.IsNullOrEmpty(responseMessage) &&
               httpResponseMessage.IsSuccessStatusCode
            ? JsonDocument.Parse(responseMessage)
            : null;
    }

    private static string GenerateSignature(JsonDocument document, string secretKey)
    {
        var parameters = document.Deserialize<SortedDictionary<string, object>>()!;

        var values = parameters.Select(p => $"{p.Key}={p.Value}");
        var original = string.Join("&", values);

        using var sha = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(original));
        return Uri.EscapeDataString(Convert.ToBase64String(hash));
    }
}