using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.Avia.Models;
#nullable enable

namespace Tgs.GameProviders.Core.Avia
{
    public interface IAviaApiClient
    {
        ValueTask<(bool IsPlayer, decimal? Balance)> BalanceAsync(Uri apiUrl, string authorization, string playerCode);

        ValueTask<(bool Success, AviaGameRecordData[]? GameRecords)> GetGameRecordAsync(Uri apiUrl, string authorization, DateTimeOffset startTime, DateTimeOffset endTime, int pageIndex = 1, int pageSize = 20);

        ValueTask<(bool IsPlayer, Uri? GameUrl)> GetGameUrlAsync(Uri apiUrl, string authorization, string playerCode, string lang, int? cateId, int? matchId);

        ValueTask KickAsync(Uri apiUrl, string authorization, string playerCode);

        ValueTask<bool> RegisterAsync(Uri apiUrl, string authorization, string playerCode, string password, string currency);

        ValueTask<(bool Success, string orderId)> TransferInAsync(Uri apiUrl, string authorization, string playerCode, string currency, decimal amount);

        ValueTask<(bool Success, string orderId)> TransferOutAsync(Uri apiUrl, string authorization, string playerCode, string currency, decimal amount);

        ValueTask<bool> TranslateinfoAsync(Uri apiUrl, string authorization, string orderId);
    }
}
