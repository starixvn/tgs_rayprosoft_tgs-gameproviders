#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Joker.Models;

namespace Tgs.GameProviders.Core.Joker;

public interface IJokerApiClient
{
    ValueTask<bool> CreateUserAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    ValueTask<(bool? IsPlayerExist, decimal? Balance)> GetBalanceAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    ValueTask<string?> GetGameTokenAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    /// <summary>
    /// FundIn (Gp -> Tgs)
    /// </summary>
    ValueTask<bool> FundInAsync(
        Uri apiUrl,
        string playerCode,
        string orderId,
        decimal amount,
        string appId,
        string secretKey);

    /// <summary>
    /// FundOut (Tgs -> Gp)
    /// </summary>
    ValueTask<bool> FundOutAsync(
        Uri apiUrl,
        string playerCode,
        string orderId,
        decimal amount,
        string appId,
        string secretKey);

    ValueTask KickPlayerAsync(Uri apiUrl, string playerCode, string appId, string secretKey);

    ValueTask<bool> FundOrderCheckingAsync(Uri apiUrl, string orderId, string appId, string secretKey);

    ValueTask<(bool Success, JokerGameRecordInfoResponse? Response)> GetGameRecordAsync(
        Uri apiUrl,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        string? nextId,
        string appId,
        string secretKey);
}