using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.WGHorse.Models;

namespace Tgs.GameProviders.Core.WGHorse;

public interface IWGHorseApiClient
{
    ValueTask<bool> AccessMoneyFundInAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount);

    ValueTask<bool> AccessMoneyFundOutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount);

    ValueTask<bool> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string orderId);

    ValueTask<(bool IsPlayer, decimal? Balance)> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);

    ValueTask<(bool Success, WGHorseGameRecord[]? GameRecord)> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, DateTimeOffset startTime, DateTimeOffset endTime, int pageIndex, int pageSize);

    ValueTask<Uri?> GetLoginAsync(Uri apiUrl, string partnerCode, string token, string lang);

    ValueTask<string?> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, long timestamp, string playerCode, string playerNickname, string currencyCode);

    ValueTask LogoutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);
}
