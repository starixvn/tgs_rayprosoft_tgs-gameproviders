namespace Tgs.GameProviders.Core;

public record GameRecordListResult
{
    public bool Success { get; init; }
    public GameRecordListModel[] Datas { get; init; }
}
