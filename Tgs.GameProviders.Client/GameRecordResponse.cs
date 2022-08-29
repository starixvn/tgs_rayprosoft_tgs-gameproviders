namespace Tgs.GameProviders.Client;

public record GameRecordResponse
{
    public bool Success { get; init; }
    public GameRecordModle[]? Datas { get; init; }
}
