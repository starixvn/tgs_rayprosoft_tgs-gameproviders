using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Client;

public interface IGameRecordListClient
{
    ValueTask<GameRecordResponse?> AviaGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<JokerGameRecordResponse?> JokerGameRecordListAsync(string brandCode, string agentId, DateTimeOffset startTime, DateTimeOffset endTime, string? nextId, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

    ValueTask<PragmaticPlayTransferGameRecordResponse?> PragmaticPlayTransferGameRecordListAsync(PragmaticPlayTransferGameDataType dataType, DateTimeOffset startTime, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> WGHorseGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> WGSportsGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);
}
