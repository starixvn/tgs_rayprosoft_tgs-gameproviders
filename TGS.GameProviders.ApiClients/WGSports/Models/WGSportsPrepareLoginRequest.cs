namespace TGS.GameProviders.ApiClients.WGSports.Models
{
    public record WGSportsPrepareLoginRequest
    {
        public long Timestamp { get; init; }
        public string PlayerCode { get; init; }
        public string PlayerNickname { get; init; }
        public string CurrencyCode { get; init; }
        public string Handicap { get; init; }
    }
}
