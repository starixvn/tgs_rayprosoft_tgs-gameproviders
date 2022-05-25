using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Client
{
    public interface IGameRecordListClient
    {
        ValueTask<GameRecordResponse?> AviaGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<JokerGameRecordResponse?> JokerGameRecordListAsync(string brandCode, string agentId, DateTimeOffset startTime, DateTimeOffset endTime, string? nextId);

        ValueTask<GameRecordResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<PragmaticPlayTransferGameRecordResponse?> PragmaticPlayTransferGameRecordListAsync(PragmaticPlayTransferGameDataType dataType, DateTimeOffset startTime);

        ValueTask<GameRecordResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime,
                    DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordResponse?> WGHorseGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordResponse?> WGSportsGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime);
    }
}