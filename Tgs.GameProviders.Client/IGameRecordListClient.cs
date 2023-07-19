namespace Tgs.GameProviders.Client;

public interface IGameRecordListClient
{
    ValueTask<GameRecordResponse?> AviaGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

    ValueTask<GameRecordResponse?> VRGameRecordListAsync(string brandCode, bool isUpdateTime, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows, CancellationToken cancellationToken = default);
}
