namespace Tgs.GameProviders.Client;

public record PlayerFundResponse(
	string OrderId,
	string GameProviderTransactionId,
	string ExternalGameProviderTransactionId);
