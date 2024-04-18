namespace Tgs.GameProviders.Client;

public record BalanceResponse(
	string PlayerCode,
	decimal Balance,
	DateTime LastDateTime);
