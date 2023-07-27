namespace Tgs.GameProviders.Core;

public record PlayerBalanceResult
{
    /// <summary>
    /// 餘額(可用餘額+佔據餘額)
    /// </summary>
    public decimal Balance => FreeBalance + OccupyBalance;
    /// <summary>
    /// 可用餘額
    /// </summary>
    public decimal FreeBalance { get; init; }
    /// <summary>
    /// 佔據餘額(遊戲商尚未結算)
    /// </summary>
    public decimal OccupyBalance { get; init; }
}

