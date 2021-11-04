namespace TGS.GameProviders.Core.WGHorse.Models
{
    public record WGHorseLoginRequest
    {
        public string Token { get; init; }
        public string Lang { get; init; }
    }
}
