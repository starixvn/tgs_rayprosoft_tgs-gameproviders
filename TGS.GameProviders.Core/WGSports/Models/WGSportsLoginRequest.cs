namespace TGS.GameProviders.Core.WGSports.Models
{
    public record WGSportsLoginRequest
    {
        public string Token { get; init; }
        public string Lang { get; init; }
    }
}
