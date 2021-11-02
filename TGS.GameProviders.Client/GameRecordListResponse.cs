namespace TGS.GameProviders.Client
{
    public record GameRecordListResponse
    {
        public bool Success { get; init; }
        public GameRecordListViewModle[]? Datas { get; init; }
    }
}
