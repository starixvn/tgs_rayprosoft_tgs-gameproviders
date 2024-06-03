namespace Tgs.GameProviders.Client;
public record PlayerOnlineResponse(
	bool GameProviderSupport,
	bool Online);