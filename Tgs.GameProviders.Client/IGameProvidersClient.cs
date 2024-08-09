using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Client;

public interface IGameProvidersClient
{
	ValueTask<Uri> GameUrlAsync(string gameProviderCode, string playerCode, int gameId, string loginRecordId, string language, GameProvidersClientPlatformType platformType, string ipAddress, Uri? backUrl, CancellationToken cancellationToken = default);

	ValueTask<PlayerLogoutResponse> PlayerLogoutAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, string playerCode, string orderId, DateTimeOffset orderOn, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundInAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundOutAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerOnlineResponse> PlayerOnlineAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default);
}
