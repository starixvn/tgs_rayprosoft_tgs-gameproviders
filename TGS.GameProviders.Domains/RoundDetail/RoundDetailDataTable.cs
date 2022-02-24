namespace TGS.GameProviders.Domains.RoundDetail
{
    public record RoundDetailDataTable : RoundDetailDataBase
    {
        public RoundDetailDataTable() : base(RoundDetailDataType.Table)
        {
        }
        public string[] Headers { get; init; } = default!;
        public List<Dictionary<string, string>> Rows { get; init; } = default!;
    }
}
