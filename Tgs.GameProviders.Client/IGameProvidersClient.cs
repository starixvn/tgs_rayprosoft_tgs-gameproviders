using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Client;

public interface IGameProvidersClient
{
	ValueTask<SM01PlatformLoginResponse> GamePlatformLoginBySM01Async(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlAsync(string gameProviderCode, GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlByAsiaGamingAsync(GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, decimal playerBalance, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlByBetConstructAsync(GameProvidersClientGameType gameType, int gameId, string brandCode, string playerCode, string currencyCode, Guid loginRecordId, GameProvidersClientPlatformType platformType, string language, string ipAddress, Uri? backUrl, string lobbyDomain, CancellationToken cancellationToken = default);

	ValueTask<PlayerLogoutResponse> PlayerLogoutAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, string playerCode, string orderId, DateTimeOffset orderOn, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundInAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundOutAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerOnlineResponse> PlayerOnlineAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default);
}
