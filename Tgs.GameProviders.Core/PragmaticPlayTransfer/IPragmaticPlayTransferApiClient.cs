#nullable enable

using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.Models;

namespace Tgs.GameProviders.Core.PragmaticPlayTransfer;

public interface IPragmaticPlayTransferApiClient
{
    ValueTask<bool> CreatePlayerAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string currencyCode);

    /// <summary>
    /// FundIn (Gp -> Tgs)
    /// </summary>
    ValueTask<bool> FundInAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount);

    ValueTask<bool> FundOrderCheckingAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string orderId);

    /// <summary>
    /// FundOut (Tgs -> Gp)
    /// </summary>
    ValueTask<bool> FundOutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string orderId,
        decimal amount);

    ValueTask<(bool? IsPlayerExist, decimal? Balance)> GetBalanceAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode);

    ValueTask<(bool Success, PragmaticPlayTransferGameRecordInfoResponse? Response)> GetGameRoundRecordsAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string dataType,
        DateTimeOffset startTime);

    ValueTask<string?> GetGameUrlAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string externalGameId,
        string language,
        string platform,
        string? lobbyUrl);

    ValueTask<string?> GetGameDetailUrlAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode,
        string externalRoundId,
        string externalGameId,
        string language);

    ValueTask LogoutAsync(
        Uri apiUrl,
        string secureLogin,
        string secretKey,
        string playerCode);
}