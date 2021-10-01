namespace TGS.GameProviders.KickPlayerReceiver.ApplicationServices
{
    public record PlayerKickCommand
    {
        public string PlayerCode { get; init; }
    }
}
