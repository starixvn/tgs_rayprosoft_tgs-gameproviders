namespace Tgs.GameProviders.Client;

public record JokerGameRecordResponse
{
    public bool Success { get; init; }
    public string? NextId { get; init; }
    public IEnumerable<GameRecordModle>? Datas { get; init; }
}
