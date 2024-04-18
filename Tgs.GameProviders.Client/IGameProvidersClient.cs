using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Client;

public interface IGameProvidersClient
{
	ValueTask<SM01PlatformLoginResponse> GamePlatformLoginBySM01Async(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlAsync(string gameProviderCode, GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlByAsiaGamingAsync(GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, decimal playerBalance, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlByBetConstructAsync(GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, string lobbyDomain, CancellationToken cancellationToken = default);

	ValueTask KickPlayerAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerFundCheckingAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerFundCheckingByVRAsync(string brandCode, string playerCode, string currencyCode, string orderId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken = default);

	ValueTask<string> PlayerFundInAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<string> PlayerFundOutAsync(string gameProviderCode, string brandCode, string playerCode, string currencyCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<RoundDetailResult> RoundDetailAsync(string gameProviderCode, Guid roundId, string language, CancellationToken cancellationToken = default);
}
