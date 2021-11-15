namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsPrepareLoginRequest
    {
        public long Timestamp { get; init; } = default!;
        public string PlayerCode { get; init; } = default!;
        public string PlayerNickname { get; init; } = default!;
        public string CurrencyCode { get; init; } = default!;
        public string Handicap { get; init; } = default!;
    }
}
