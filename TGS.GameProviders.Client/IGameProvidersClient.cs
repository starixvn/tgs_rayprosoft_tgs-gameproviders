using System;
using System.Threading.Tasks;

namespace TGS.GameProviders.Client
{
    public interface IGameProvidersClient
    {
        ValueTask<GameUrlResponse?> GameURLAsync(string gameProviderCode, GameUrlRequest source);

        ValueTask KickPlayerAsync(string gameProviderCode, KickPlayerRequest source);

        ValueTask<PlayerBalanceResponse?> PlayerBalanceAsync(string gameProviderCode, PlayerBalanceRequest source);

        ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, PlayerFundCheckingRequest source);

        ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, VRPlayerFundCheckingRequest source);

        ValueTask<PlayerFundResponse?> PlayerFundInAsync(string gameProviderCode, PlayerFundRequest source);

        ValueTask<PlayerFundResponse?> PlayerFundOutAsync(string gameProviderCode, PlayerFundRequest source);

        ValueTask<GameRecordListResponse?> BigGamingFishingGameRecordListAsync(string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordListResponse?> KYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordListResponse?> LYGameRecordListAsync(string brandCode, string currencyCode, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordListResponse?> VRGameRecordListAsync(string brandCode, DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

        ValueTask<GameRecordListResponse?> WGFMGameRecordListAsync(bool isSettlement, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<GameRecordListResponse?> WGVIAGameRecordListAsync(DateTimeOffset startTime, DateTimeOffset endTime, int page, int rows);

    }
}
