namespace TGS.GameProviders.Core.Saba.ApplicationServices
{
    public record SabaKickPlayerCommand : KickPlayerCommand
    {
        public string EnvironmentName { get; init; } = default!;
    }
}
