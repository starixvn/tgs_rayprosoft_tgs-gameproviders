#nullable enable

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.PragmaticPlay;
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

    public ValueTask<bool> CreatePlayerAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string currencyCode)
    {
        throw new NotImplementedException();
    }

    public ValueTask<(bool Success, string TransactionId)> FundInAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount)
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> FundOrderCheckingAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string orderId)
    {
        throw new NotImplementedException();
    }

    public ValueTask<(bool Success, string TransactionId)> FundOutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount)
    {
        throw new NotImplementedException();
    }

    public ValueTask<(bool IsPlayerExist, decimal? Balance)> GetBalanceAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string> GetGameRoundRecordsAsync(
        Uri apiUrl, string secureLogin,
        string secretKey,
        string dataType,
        DateTimeOffset startTime)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetRoundHistoryAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string roundId,
        string language)
    {
        throw new NotImplementedException();
    }

    public ValueTask LogoutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode)
    {
        throw new NotImplementedException();
    }

    public ValueTask<string?> GetGameUrlAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string externalGameId,
        string language,
        string platform,
        string? lobbyUrl)
    {
        throw new NotImplementedException();
    }
}