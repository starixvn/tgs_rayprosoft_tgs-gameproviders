namespace TGS.GameProviders.Core
{
    public record GameRecordListResult
    {
        public bool Success { get; init; }
        public GameRecordListModle[] Datas { get; init; }
    }
}
