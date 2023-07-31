namespace Tgs.GameProviders.ViewModels;

public record PlayerBalanceResponse(
    decimal Balance,
    decimal FreeBalance,
    decimal OutstandingBalance);