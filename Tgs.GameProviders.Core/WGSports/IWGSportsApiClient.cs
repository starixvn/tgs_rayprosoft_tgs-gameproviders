using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.WGSports.Models;

namespace Tgs.GameProviders.Core.WGSports;

public interface IWGSportsApiClient
{
    ValueTask<bool> AccessMoneyFundInAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount);

    ValueTask<bool> AccessMoneyFundOutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode, string orderId, decimal amount);

    ValueTask<bool> AccessMoneyQueryAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string orderId);

    ValueTask<(bool IsPlayer, decimal? Balance)> GetBalanceAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);

    ValueTask<(bool Success, WGSportsBetInfo[]? BetInfos)> GetGameRecordAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, GameRecordTime gameRecordTime, string startTime, string endTime);

    ValueTask<Uri?> GetLoginAsync(Uri apiUrl, string partnerCode, string token, string lang);

    ValueTask<string?> GetPrepareLoginAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, long timestamp, string playerCode, string playerNickname, string currencyCode, string handicap);

    ValueTask LogoutAsync(Uri apiUrl, string partnerCode, string partnerKey, string partnerIV, string playerCode);
}
