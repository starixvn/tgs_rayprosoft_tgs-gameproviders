namespace TGS.GameProviders.Core.KY.ApplicationServices
{
    public record KYGameRecordListResult
    {
        public bool Success { get; init; }
        public KYGameRecordListModle[] Datas { get; init; }
    }
}
