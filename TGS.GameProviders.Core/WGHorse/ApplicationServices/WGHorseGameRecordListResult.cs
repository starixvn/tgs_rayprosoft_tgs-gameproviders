namespace TGS.GameProviders.Core.WGHorse.ApplicationServices
{
    public record WGHorseGameRecordListResult
    {
        public bool Success { get; init; }
        public GameRecordListModle[] Datas { get; init; }
    }
}
