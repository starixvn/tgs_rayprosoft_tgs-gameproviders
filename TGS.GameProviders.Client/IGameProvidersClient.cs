using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.RoundDetail;

namespace TGS.GameProviders.Client
{
    public interface IGameProvidersClient
    {
        ValueTask<Uri?> GameUrlAsync(string gameProviderCode, GameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, PlatformType platformType, string language, string ipAddress, Uri? backUrl);

        ValueTask KickPlayerAsync(string gameProviderCode, string playerCode);

        ValueTask<decimal> PlayerBalanceAsync(string gameProviderCode, string playerCode);

        ValueTask<bool?> PlayerFundCheckingAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId);

        ValueTask<bool?> PlayerFundCheckingByVRAsync(string brandCode, string playerCode, string currencyCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime);

        ValueTask<string?> PlayerFundInAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount);

        ValueTask<string?> PlayerFundOutAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount);

        ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, string language);
    }
}
