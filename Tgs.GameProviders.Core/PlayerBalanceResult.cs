namespace Tgs.GameProviders.Core;

public record PlayerBalanceResult
{
    /// <summary>
    /// 餘額(可用餘額+未結餘額)
    /// </summary>
    public decimal Balance => FreeBalance + OutstandingBalance;
    /// <summary>
    /// 可用餘額
    /// </summary>
    public decimal FreeBalance { get; init; } = 0;
    /// <summary>
    /// 未結餘額(遊戲商未結算)
    /// </summary>
    public decimal OutstandingBalance { get; init; } = 0;
}

