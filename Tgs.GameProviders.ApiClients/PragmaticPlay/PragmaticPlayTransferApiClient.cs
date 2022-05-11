#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Tgs.GameProviders.Core.PragmaticPlayTransfer;
using Tgs.Logging;

namespace Tgs.GameProviders.ApiClients.PragmaticPlay;

internal class PragmaticPlayTransferApiClient : IPragmaticPlayTransferApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<PragmaticPlayTransferApiClient> _logger;
    private readonly ITgsLog _tgsLog;

    public PragmaticPlayTransferApiClient(
        HttpClient client,
        ILogger<PragmaticPlayTransferApiClient> logger,
        ITgsLog tgsLog)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
    }

    public async ValueTask<bool> CreatePlayerAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string currencyCode)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode,
            ["currency"] = currencyCode
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
            apiUrl,
            "/IntegrationService/v3/http/CasinoGameAPI/player/account/create/",
            parameters,
            "CreatePlayer")
            .ConfigureAwait(false);

        var errorCode = document?.RootElement.GetProperty("error").GetString();
        return errorCode is "0";
    }

    public async ValueTask<bool> FundInAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode,
            ["externalTransactionId"] = orderId,
            ["amount"] = (-amount).ToString()
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/balance/transfer/",
                parameters,
                " FundIn")
            .ConfigureAwait(false);

        var error = document?.RootElement.GetProperty("error").GetString();
        return error is "0" or "8";
    }

    public async ValueTask<bool> FundOrderCheckingAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string orderId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalTransactionId"] = orderId
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/balance/transfer/status/",
                parameters,
                "FundOrderChecking")
            .ConfigureAwait(false);

        if (document is null)
            return false;

        var error = document.RootElement.GetProperty("error").GetInt32();
        if (error != 0)
            return false;

        var status = document.RootElement.GetProperty("status").GetString();
        return status == "Success";
    }

    public async ValueTask<bool> FundOutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode,
            ["externalTransactionId"] = orderId,
            ["amount"] = amount.ToString()
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/balance/transfer/",
                parameters,
                "FundOut")
            .ConfigureAwait(false);

        var error = document?.RootElement.GetProperty("error").GetString();
        return error is "0" or "8";
    }

    public async ValueTask<(bool? IsPlayerExist, decimal? Balance)> GetBalanceAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode,
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/balance/current/",
                parameters,
                "GetBalance")
            .ConfigureAwait(false);

        var error = document?.RootElement.GetProperty("error").GetString();

        switch (error)
        {
            case "0":
                {
                    var balance = document?.RootElement.GetProperty("balance").GetDecimal();
                    return (true, balance);
                }
            case "17":
                return (false, null);

            default:
                return (null, null);
        }
    }

    public ValueTask<string?> GetGameRoundRecordsAsync(
        Uri apiUrl, string secureLogin,
        string secretKey,
        string dataType,
        DateTimeOffset startTime)
    {
        var queryStrings = HttpUtility.ParseQueryString(string.Empty);
        queryStrings.Add("login", secureLogin);
        queryStrings.Add("password", secretKey);
        queryStrings.Add("dataType", dataType);
        queryStrings.Add("timepoint", startTime.ToUnixTimeMilliseconds().ToString());

        return GetCsvAsync(
            apiUrl,
            "/IntegrationService/v3/DataFeeds/gamerounds/",
            queryStrings,
            "GameRounds");
    }

    public async ValueTask<string?> GetGameUrlAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string externalGameId,
        string language,
        string platform,
        string? lobbyUrl)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode,
            ["gameId"] = externalGameId,
            ["language"] = language,
            ["platform"] = platform
        };

        if (!string.IsNullOrWhiteSpace(lobbyUrl))
            parameters["lobbyURL"] = lobbyUrl;

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/game/start/",
                parameters,
                "StartGame")
            .ConfigureAwait(false);

        var errorCode = document?.RootElement.GetProperty("error").GetString();
        if (errorCode is not "0")
            return null;

        var gameUrl = document!.RootElement.GetProperty("gameURL").GetString();
        return gameUrl;
    }

    public async ValueTask<string?> GetRoundHistoryAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string externalRoundId,
        string externalGameId,
        string language)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["playerId"] = playerCode,
            ["gameId"] = externalGameId,
            ["roundId"] = externalRoundId,
            ["language"] = language
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        var document = await PostAsync(
            apiUrl,
            "/IntegrationService/v3/http/HistoryAPI/OpenHistoryExtended/",
            parameters,
            "RoundHistory");

        var errorCode = document?.RootElement.GetProperty("error").GetString();
        if (errorCode is not "0")
            return null;

        var url = document!.RootElement.GetProperty("url").GetString();
        return url;
    }

    public async ValueTask LogoutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode)
    {
        var parameters = new Dictionary<string, string>
        {
            ["secureLogin"] = secureLogin,
            ["externalPlayerId"] = playerCode
        };

        var hash = GenerateHash(parameters, secretKey);
        parameters["hash"] = hash;

        _ = await PostAsync(
                apiUrl,
                "/IntegrationService/v3/http/CasinoGameAPI/game/session/terminate/",
                parameters,
            "Logout")
            .ConfigureAwait(false);
    }

    private static string GenerateHash(IDictionary<string, string> parameters, string secretKey)
    {
        var sortedParameters = new SortedDictionary<string, string>(parameters);
        var values = sortedParameters.Select(p => $"{p.Key}={p.Value}");

        var original = $"{string.Join("&", values)}{secretKey}";

        using var hasher = MD5.Create();
        var data = Encoding.UTF8.GetBytes(original);

        var hash = hasher.ComputeHash(data);
        return hash.ToHexString();
    }

    private async ValueTask<JsonDocument?> PostAsync(
        Uri apiUrl,
        string path,
        IEnumerable<KeyValuePair<string, string>> parameters,
        string action)
    {
        var logPrefix = $"{nameof(PragmaticPlayTransferApiClient).ToLower()}.{action}";
        var uriString = $"{apiUrl.OriginalString.TrimEnd('/')}{path}";

        var sw = Stopwatch.StartNew();

        var content = new FormUrlEncodedContent(parameters);
        var httpResponseMessage = await _client.PostAsync(uriString, content).ConfigureAwait(false);

        sw.Stop();

        string responseMessage;

        var requestMessage = await content.ReadAsStringAsync().ConfigureAwait(false);

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
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers
                             + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = requestMessage.NoLongerThan(4096),
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

        Console.WriteLine(JsonSerializer.Serialize(apiLogEvent));

        return !string.IsNullOrEmpty(responseMessage) &&
               httpResponseMessage.StatusCode == HttpStatusCode.OK
            ? JsonDocument.Parse(responseMessage)
            : default;
    }

    private async ValueTask<string?> GetCsvAsync(
        Uri apiUrl,
        string path,
        NameValueCollection parameters,
        string action)
    {
        var logPrefix = $"{nameof(PragmaticPlayApiClient).ToLower()}.{action}";
        var uriString = $"{apiUrl.OriginalString.TrimEnd('/')}{path}?{parameters}";

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
            Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
            Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
            Prefix = logPrefix,
            ServerName = Environment.MachineName.NoLongerThan(128),
            RequestHeaders = httpResponseMessage.RequestMessage?.Headers
                             + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
            RequestContent = string.Empty,
            ResponseHeaders = httpResponseMessage.Headers.ToString(),
            ResponseContent = responseMessage.NoLongerThan(4096),
            ResponseCode = (short)httpResponseMessage.StatusCode,
            ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
            RecordedOn = DateTimeOffset.UtcNow
        };

        _tgsLog.LogApi(apiLogEvent);
        _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

        return !string.IsNullOrEmpty(responseMessage) &&
               httpResponseMessage.StatusCode == HttpStatusCode.OK
            ? responseMessage
            : default;
    }
}