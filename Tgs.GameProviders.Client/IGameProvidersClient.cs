using Tgs.GameProviders.Domains;

using Tgs.GameProviders.Domains.RoundDetail;

namespace Tgs.GameProviders.Client;

public interface IGameProvidersClient
{
	ValueTask<GameUrlResponse?> TicketDetailUrlAsync(string gameProviderCode, string playerCode, long refId, string language = "en-US", string? baseCurrency = null, string? playerUsername = null, CancellationToken cancellationToken = default);

	ValueTask<Uri> GameUrlAsync(string gameProviderCode, string playerCode, int gameId, string loginRecordId, string language, PlatformType platformType, string ipAddress, Uri? backUrl, object? extraInfo, CancellationToken cancellationToken = default);

	ValueTask<PlayerLogoutResponse> PlayerLogoutAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceResponse> PlayerBalanceAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundCheckingResponse?> PlayerFundCheckingAsync(string gameProviderCode, string playerCode, string orderId, DateTimeOffset orderOn, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundInAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerFundResponse> PlayerFundOutAsync(string gameProviderCode, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<PlayerOnlineResponse> PlayerOnlineAsync(string gameProviderCode, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<RoundDetailResult?> RoundDetailAsync(string gameProviderCode, Guid roundId, DateTimeOffset externalCreatedOn, string language, CancellationToken cancellationToken = default);

	ValueTask<TResult> BetResultCheck<TResult>(string gameProviderCode, string externalGameId, string externalRoundId, string playerCode, CancellationToken cancellationToken = default);
}
