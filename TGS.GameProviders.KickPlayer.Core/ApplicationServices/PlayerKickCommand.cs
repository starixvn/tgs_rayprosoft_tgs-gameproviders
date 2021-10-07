namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices
{
    public record PlayerKickCommand
    {
        public string PlayerCode { get; init; }
    }
}
