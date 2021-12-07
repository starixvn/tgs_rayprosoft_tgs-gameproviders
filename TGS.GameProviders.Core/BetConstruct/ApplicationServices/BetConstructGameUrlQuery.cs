namespace TGS.GameProviders.Core.BetConstruct.ApplicationServices
{
    public record BetConstructGameUrlQuery : GameUrlQuery
    {
        public string LobbyDomain { get; set; }        
    }
}