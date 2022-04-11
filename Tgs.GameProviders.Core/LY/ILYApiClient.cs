using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Core.LY.Models;

namespace Tgs.GameProviders.Core.LY
{
    public interface ILYApiClient
    {
        ValueTask<bool> FundCheckingAsync(Uri apiUrl, string deskey, string md5key, string agentId, string orderId);

        ValueTask<(bool Success, string orderId)> FundInAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, decimal amount);

        ValueTask<(bool Success, string orderId)> FundOutAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, decimal amount);

        ValueTask<decimal?> GetBalanceAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode);

        ValueTask<(bool Success, LYGameRecordList GameRecordList)> GetGameRecordAsync(Uri apiRecordUrl, string deskey, string md5key, string agentId, long startTime, long endTime);

        ValueTask<Uri> GetGameUrlAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode, string ipAddress, string brandCode, int gameExternalID);

        ValueTask KickAsync(Uri apiUrl, string deskey, string md5key, string agentId, string playerCode);
    }
}
