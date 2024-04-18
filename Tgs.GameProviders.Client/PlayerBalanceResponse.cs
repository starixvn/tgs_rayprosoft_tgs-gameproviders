namespace Tgs.GameProviders.Client;

public record PlayerBalanceResponse(
	decimal Balance,
	decimal FreeBalance,
	decimal OutstandingBalance);
