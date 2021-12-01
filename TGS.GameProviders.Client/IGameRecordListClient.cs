using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IGameRecordListClient
    {
        ValueTask<GameRecordListResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordListResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordListResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordListResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordListResponse?> WGHorseGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordListResponse?> WGSportsGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime);
    }
}
